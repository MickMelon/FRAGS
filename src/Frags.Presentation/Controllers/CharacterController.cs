using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Progression;
using Frags.Core.Statistics;
using Frags.Presentation.Results;

namespace Frags.Presentation.Controllers
{
    /// <summary>
    /// This class controls character related actions.
    /// </summary>
    public class CharacterController
    {
        /// <summary>
        /// Used to interact with the character database.
        /// </summary>
        private readonly ICharacterProvider _provider;

        /// <summary>
        /// Used to calculate experience points given.
        /// </summary>
        private readonly IProgressionStrategy _progStrategy;

        /// <summary>
        /// Used to determine the character limit.
        /// </summary>
        private readonly GeneralOptions _options;

        private readonly ICampaignProvider _campProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterController" /> class.
        /// </summary>
        /// <param name="provider">The CharacterProvider.</param>
        public CharacterController(ICharacterProvider provider, IProgressionStrategy progStrategy, GeneralOptions options, ICampaignProvider campProvider = null)
        {
            _provider = provider;
            _progStrategy = progStrategy;
            _options = options;
            _campProvider = campProvider;
        }

        /// <summary>
        /// Gets the caller's active character and returns the result.
        /// </summary> 
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <returns>A new CharacterResult object.</returns>
        public async Task<IResult> ShowCharacterAsync(ulong callerId)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();
            return CharacterResult.Show(character, _progStrategy.GetCharacterLevel(character), await _progStrategy.GetCharacterInfo(character));
        }

        public async Task<IResult> ListCharactersAsync(ulong callerId)
        {
            var characters = await _provider.GetAllCharactersAsync(callerId);
            return GenericResult.Generic(string.Join("\n", characters.OrderBy(x => x.Id).Select(x => x.Name)));
        }

        /// <summary>
        /// Gets the caller's specified character by name, sets it as active, and updates it.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="charName">The name of the character to set as active.</param>
        /// <returns>A new CharacterResult object.</returns>
        public async Task<IResult> ActivateCharacterAsync(ulong callerId, string charName)
        {
            var characters = await _provider.GetAllCharactersAsync(callerId);

            var match = characters.OrderBy(x => x.Id).FirstOrDefault(x => x.Name.ContainsIgnoreCase(charName));

            if (match == null) return CharacterResult.CharacterNotFound();
            if (match.Active) return CharacterResult.CharacterAlreadyActive();
            match.Active = true;

            await _provider.UpdateCharacterAsync(match);
            return CharacterResult.CharacterActive();
        }

        public async Task<IResult> RenameCharacterAsync(ulong id, string newName)
        {
            var character = await _provider.GetActiveCharacterAsync(id);
            if (character == null) return CharacterResult.CharacterNotFound();

            character.Name = newName;
            await _provider.UpdateCharacterAsync(character);
            
            return CharacterResult.CharacterUpdatedSuccessfully();
        }

        /// <summary>
        /// Creates a new character.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="name">The desired character name.</param>
        /// <returns>A new CharacterResult object.</returns>
        public async Task<IResult> CreateCharacterAsync(ulong callerId, string name)
        {
            var characters = await _provider.GetAllCharactersAsync(callerId);
            if (characters != null && characters.Count >= _options.CharacterLimit) return CharacterResult.TooManyCharacters();

            var existing = characters?.Where(c => c.Name.EqualsIgnoreCase(name)).FirstOrDefault();
            if (existing != null) return CharacterResult.NameAlreadyExists();

            await _provider.CreateCharacterAsync(callerId, name);
            return CharacterResult.CharacterCreatedSuccessfully();
        }

        public async Task<IResult> GiveMoneyAsync(ulong callerId, int money)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();
            
            character.Money += money;
            await _provider.UpdateCharacterAsync(character);
            return CharacterResult.CharacterUpdatedSuccessfully();
        }

        public async Task<IResult> GiveMoneyToOtherAsync(ulong callerId, ulong targetId, int money)
        {
            var caller = await _provider.GetActiveCharacterAsync(callerId);
            if (caller == null) return CharacterResult.CharacterNotFound();

            var target = await _provider.GetActiveCharacterAsync(targetId);
            if (target == null) return CharacterResult.CharacterNotFound();
            
            if (caller.Money - money >= 0)
            {
                caller.Money -= money;
                target.Money += money;

                await _provider.UpdateCharacterAsync(caller);
                await _provider.UpdateCharacterAsync(target);

                return CharacterResult.CharacterUpdatedSuccessfully();
            }
            else
            {
                return GenericResult.ValueTooHigh();
            }
        }

        /// <summary>
        /// Gets the caller's specified character by name, sets its description, and updates it.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="desc">The new description of the character.</param>
        /// <returns>A new CharacterResult object.</returns>
        public async Task<IResult> SetCharacterDescriptionAsync(ulong callerId, string desc)
        {
            if (string.IsNullOrEmpty(desc)) return GenericResult.InvalidInput();

            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();
            character.Description = desc;

            await _provider.UpdateCharacterAsync(character);
            return CharacterResult.CharacterUpdatedSuccessfully();
        }

        /// <summary>
        /// Gets the caller's specified character by name, sets its story, and updates it.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="story">The new story of the character.</param>
        /// <returns>A new CharacterResult object.</returns>
        public async Task<IResult> SetCharacterStoryAsync(ulong callerId, string story)
        {
            if (string.IsNullOrEmpty(story)) return GenericResult.InvalidInput();

            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();
            character.Story = story;

            await _provider.UpdateCharacterAsync(character);
            return CharacterResult.CharacterUpdatedSuccessfully();
        }

        /// <summary>
        /// Gives experience points to a character determined by configuration.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="channelId">Channel ID of where the message orignated from.</param>
        /// <param name="message">Message to calculate experience points from.</param>
        public async Task<bool> GiveExperienceAsync(ulong callerId, ulong channelId, string message)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return false;

            bool result = await _progStrategy.AddExperienceFromMessage(character, channelId, message);
            _ = _provider.UpdateCharacterAsync(character);
            return result;
        }
    }
}
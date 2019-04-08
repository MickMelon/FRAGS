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
        /// Initializes a new instance of the <see cref="CharacterController" /> class.
        /// </summary>
        /// <param name="provider">The CharacterProvider.</param>
        public CharacterController(ICharacterProvider provider, IProgressionStrategy progStrategy)
        {
            _provider = provider;
            _progStrategy = progStrategy;
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
            return CharacterResult.Show(character, _progStrategy.GetCharacterLevel(character));
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

            var match = characters.FirstOrDefault(x => x.Name.ContainsIgnoreCase(charName));

            if (match == null) return CharacterResult.CharacterNotFound();
            if (match.Active) return CharacterResult.CharacterAlreadyActive();
            match.Active = true;

            await _provider.UpdateCharacterAsync(match);
            return CharacterResult.CharacterActive();
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
            var existing = characters.Where(c => c.Name.EqualsIgnoreCase(name)).FirstOrDefault();
            if (existing != null) return CharacterResult.NameAlreadyExists();

            await _provider.CreateCharacterAsync(callerId, name);
            return CharacterResult.CharacterCreatedSuccessfully();
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

            bool result = await _progStrategy.AddExperience(character, channelId, message);
            _ = _provider.UpdateCharacterAsync(character);
            return result;
        }
    }
}
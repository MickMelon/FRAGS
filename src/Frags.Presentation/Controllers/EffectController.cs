using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Common.Exceptions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Progression;
using Frags.Core.Effects;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels;

namespace Frags.Presentation.Controllers
{
    public class EffectController
    {
        /// <summary>
        /// Used to interact with the character database.
        /// </summary>
        private readonly ICharacterProvider _charProvider;

        /// <summary>
        /// Used to interact with the Effect database.
        /// </summary>
        private readonly IEffectProvider _effectProvider;

        public EffectController(ICharacterProvider charProvider, IEffectProvider statProvider)
        {
            _charProvider = charProvider;
            _effectProvider = statProvider;
        }

        /// <summary>
        /// Creates a new Effect in the database.
        /// </summary>
        /// <param name="effectName">The name for the new effect.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CreateEffectAsync(string effectName)
        {
            if (await _effectProvider.GetEffectAsync(effectName) != null)
                return EffectResult.NameAlreadyExists();

            var result = await _effectProvider.CreateEffectAsync(effectName);
            if (result == null) return EffectResult.EffectCreationFailed();
            return EffectResult.EffectCreatedSuccessfully();
        }

        /// <summary>
        /// Renames an already existing Effect.
        /// </summary>
        /// <param name="effectName">The name of the Effect to rename.</param>
        /// <param name="newName">The new name of the Effect.</param>
        /// <returns>A result detailing if the operation was successful or why it failed.</returns>
        /// <remarks>This method will also clear its aliases.</remarks>
        public async Task<IResult> RenameEffectAsync(string effectName, string newName)
        {
            var stat = await _effectProvider.GetEffectAsync(effectName);
            if (stat == null) return EffectResult.EffectNotFound();

            if (await _effectProvider.GetEffectAsync(newName) != null)
                return EffectResult.NameAlreadyExists();

            stat.Name = newName;
            await _effectProvider.UpdateEffectAsync(stat);

            return EffectResult.EffectUpdatedSucessfully();
        }

        /// <summary>
        /// Sets an effect's description.
        /// </summary>
        /// <param name="effectName">The name of the effect to set the description to.</param>
        /// <param name="desc">The new description of the effect.</param>
        /// <returns>A result detailing if the operation was successful or why it failed.</returns>
        public async Task<IResult> SetDescriptionAsync(string effectName, string desc)
        {
            var stat = await _effectProvider.GetEffectAsync(effectName);
            if (stat == null) return EffectResult.EffectNotFound();

            stat.Description = desc;
            await _effectProvider.UpdateEffectAsync(stat);

            return EffectResult.EffectUpdatedSucessfully();
        }

        /// <summary>
        /// Deletes a Effect in the database.
        /// </summary>
        /// <param name="statName">The name for the new skill.</param>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> DeleteEffectAsync(string statName)
        {
            var Effect = await _effectProvider.GetEffectAsync(statName);
            if (Effect == null)
                return EffectResult.EffectNotFound();

            await _effectProvider.DeleteEffectAsync(Effect);
            return EffectResult.EffectDeletedSuccessfully();
        }

        /// <summary>
        /// Gets the caller's active character and returns its active effects.
        /// </summary> 
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <returns>A new EffectResult object.</returns>
        public async Task<IResult> ShowCharacterEffectsAsync(ulong callerId)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            if (character.EffectMappings == null || character.EffectMappings.Count <= 0)
                return EffectResult.EffectNotFound();

            return EffectResult.ShowCharacterEffects(character);
        }

        /// <summary>
        /// Used to add an effect to a character.
        /// </summary>
        /// <param name="callerId">The user identifier of the caller.</param>
        /// <param name="effectName">The name of the effect to add to the character.</param>
        public async Task<IResult> AddEffectAsync(ulong callerId, string effectName)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            var effect = await _effectProvider.GetEffectAsync(effectName);
            if (effect == null) return EffectResult.EffectNotFound();

            if (character.EffectMappings.Count(x => x.Effect.Equals(effect)) > 0)
                return EffectResult.EffectAlreadyAdded();

            character.EffectMappings.Add(new EffectMapping { Effect = effect, Character = character });
            return EffectResult.EffectAdded();
        }

        /// <summary>
        /// Used to remove an effect from a character.
        /// </summary>
        /// <param name="callerId">The user identifier of the caller.</param>
        /// <param name="effectName">The name of the effect to remove from the character.</param>
        public async Task<IResult> RemoveEffectAsync(ulong callerId, string effectName)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            var effect = await _effectProvider.GetEffectAsync(effectName);
            if (effect == null) return EffectResult.EffectNotFound();

            var match = character.EffectMappings.FirstOrDefault(x => x.Effect.Equals(effect));

            if (match == null)
                return EffectResult.EffectNotFound();

            character.EffectMappings.Remove(match);
            await _charProvider.UpdateCharacterAsync(character);

            return EffectResult.EffectRemoved();
        }
    }
}
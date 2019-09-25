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
using Frags.Core.Statistics;

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

        private readonly IStatisticProvider _statProvider;

        /// <summary>
        /// Used to determine the limit of effects for one user.
        /// </summary>
        private readonly GeneralOptions _options;

        public EffectController(ICharacterProvider charProvider, 
            IEffectProvider effectProvider, 
            IStatisticProvider statProvider,
            GeneralOptions options)
        {
            _charProvider = charProvider;
            _effectProvider = effectProvider;
            _statProvider = statProvider;
            _options = options;
        }

        /// <summary>
        /// Creates a new Effect in the database.
        /// </summary>
        /// <param name="effectName">The name for the new effect.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CreateEffectAsync(ulong callerId, string effectName)
        {
            if (await _effectProvider.GetEffectAsync(effectName) != null)
                return EffectResult.NameAlreadyExists();

            if ((await _effectProvider.GetUserEffectsAsync(callerId)).Count() >= _options.EffectsLimit)
                return EffectResult.TooManyEffects();

            var result = await _effectProvider.CreateEffectAsync(callerId, effectName);

            if (result == null) return EffectResult.EffectCreationFailed();
            return EffectResult.EffectCreatedSuccessfully();
        }

        /// <summary>
        /// Returns a result containing a string of effects created by the given user.
        /// </summary>
        /// <param name="id">The user identifier to show effects created by.</param>
        /// <returns>A GenericResult with a Message property containing the user's created effects.</returns>
        public async Task<IResult> ListCreatedEffectsAsync(ulong callerId)
        {
            var effects = await _effectProvider.GetUserEffectsAsync(callerId);
            return GenericResult.Generic(string.Join("\n", effects.OrderBy(x => x.Id).Select(x => x.Name)));
        }

        /// <summary>
        /// Sets the statistic effects of the specified effect.
        /// </summary>
        /// <param name="effectName">The name of the effect to set the value to.</param>
        /// <param name="statName">The name of the statistic to associate the value with.</param>
        /// <param name="value">The value to add (or subtract) to the statistic.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> SetStatisticEffectAsync(string effectName, string statName, int value)
        {
            var effect = await _effectProvider.GetEffectAsync(effectName);
            if (effect == null) return EffectResult.EffectNotFound();

            var stat = await _statProvider.GetStatisticAsync(statName);
            if (stat == null) return StatisticResult.StatisticNotFound();

            var match = effect.StatisticEffects.FirstOrDefault(x => x.Statistic.Equals(stat));

            if (match == null)
                effect.StatisticEffects.Add(new StatisticMapping(stat, new StatisticValue(value)));
            else
                match.StatisticValue.Value = value;

            await _effectProvider.UpdateEffectAsync(effect);
            return EffectResult.EffectUpdatedSucessfully();
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

            if (character.Effects == null || character.Effects.Count <= 0)
                return EffectResult.NoEffects();

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

            if (character.Effects == null)
                character.Effects = new List<Effect>();

            if (character.Effects.Count(x => x.Id == effect.Id) > 0)
                return EffectResult.EffectAlreadyAdded();

            character.Effects.Add(effect);
            await _charProvider.UpdateCharacterAsync(character);
            
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

            var match = character.Effects.FirstOrDefault(x => x.Id == effect.Id);

            if (match == null)
                return EffectResult.EffectNotFound();

            character.Effects.Remove(match);
            await _charProvider.UpdateCharacterAsync(character);

            return EffectResult.EffectRemoved();
        }
    }
}
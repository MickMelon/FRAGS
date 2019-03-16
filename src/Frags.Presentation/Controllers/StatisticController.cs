using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Core.Statistics;
using Frags.Presentation.Results;

namespace Frags.Presentation.Controllers
{
    public class StatisticController
    {
        /// <summary>
        /// Used to interact with the character database.
        /// </summary>
        private readonly ICharacterProvider _charProvider;

        /// <summary>
        /// Used to interact with the statistic database.
        /// </summary>
        private readonly IStatisticProvider _statProvider;

        /// <summary>
        /// Used to validate input when setting initial statistics.
        /// </summary>
        private readonly StatisticOptions _statOptions;

        public StatisticController(ICharacterProvider charProvider, IStatisticProvider statProvider, StatisticOptions statOptions)
        {
            _charProvider = charProvider;
            _statProvider = statProvider;
            _statOptions = statOptions;
        }

        /// <summary>
        /// Used to set a character's attributes up.
        /// </summary>
        /// <param name="callerId">The user identifier of the caller.</param>
        /// <param name="values">What to set the initial attributes to.</param>
        public async Task<IResult> SetAttributeAsync(ulong callerId, string statName, int newValue)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();
            if (character.Level > _statOptions.InitialSetupMaxLevel && await InitialAttributesSet(character)) return CharacterResult.LevelTooHigh();

            var statistic = await _statProvider.GetStatisticAsync(statName);
            if (!(statistic is Attribute)) return StatisticResult.StatisticNotFound();

            if (newValue < _statOptions.InitialAttributeMin) return GenericResult.ValueTooLow();
            if (newValue > _statOptions.InitialAttributeMax) return GenericResult.ValueTooHigh();

            // Get an IEnumerable of the character's current attributes
            var attribs = character.Statistics.Where(x => x.Key is Attribute).ToDictionary(x => (Attribute)x.Key, x => x.Value);
            var sum = attribs.Sum(x => x.Value.Value);

            StatisticValue currentVal = new StatisticValue(0);
            // Save the old value for later
            bool containsStat = false;
            if (character.Statistics.ContainsKey(statistic))
            {
                currentVal = character.Statistics[statistic];
                containsStat = true;
            }

            // Make sure the character has enough remaining points to do that (we refund the current stat value since we're overwriting it)
            if (_statOptions.InitialAttributePoints - (sum - currentVal.Value + newValue) < 0) return GenericResult.NotEnoughPoints();

            // Check if they go over the limit for attributes set to the max
            // Example: InitialAttributesAtMax is set to 2 and InitialAttributeMax is set to 10
            // If we already have 2 attributes with a value of 10 and we try to set a third, disallow it.
            if (_statOptions.InitialAttributesAtMax > 0 &&
                newValue == _statOptions.InitialAttributeMax &&
                    attribs.Count(x => x.Value.Value == _statOptions.InitialAttributeMax) + 1 > _statOptions.InitialAttributesAtMax)
                        return StatisticResult.TooManyAtMax(_statOptions.InitialAttributesAtMax);

            if (containsStat)
                character.Statistics[statistic] = new StatisticValue(newValue);
            else
                character.Statistics.Add(statistic, new StatisticValue(newValue));

            await _charProvider.UpdateCharacterAsync(character);
            return StatisticResult.StatisticSetSucessfully();
        }

        /// <summary>
        /// Checks if a character's starting attributes have been set.
        /// </summary>
        private async Task<bool> InitialAttributesSet(Character character)
        {
            if (character == null || character.Statistics == null) return false;

            var attribs = character.Statistics.Where(x => x.Key is Attribute).ToDictionary(x => (Attribute)x.Key, x => x.Value);
            var sum = attribs.Sum(x => x.Value.Value);

            // Character attributes don't match up with database attributes
            if (attribs.Count != (await _statProvider.GetAllStatisticsAsync()).OfType<Attribute>().Count()) return false;
            // Character has not set their initial attribute values
            if (sum < _statOptions.InitialAttributePoints) return false;
            
            return true;
}
    }
}
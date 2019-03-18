using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Core.Game.Statistics;
using Frags.Core.Statistics;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels;

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
        /// Creates a new Attribute in the database.
        /// </summary>
        /// <param name="statName">The name for the new attribute.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CreateAttributeAsync(string statName)
        {
            if (await _statProvider.GetStatisticAsync(statName) != null)
                return StatisticResult.NameAlreadyExists();

            var result = await _statProvider.CreateAttributeAsync(statName);
            if (result == null) return StatisticResult.StatisticCreationFailed();
            return StatisticResult.StatisticCreatedSuccessfully();
        }

        /// <summary>
        /// Creates a new Skill in the database.
        /// </summary>
        /// <param name="statName">The name for the new skill.</param>
        /// <param name="attribName">The name of the attribute to go with the skill. Must exist in the database beforehand.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CreateSkillAsync(string statName, string attribName)
        {
            if (await _statProvider.GetStatisticAsync(statName) != null)
                return StatisticResult.NameAlreadyExists();

            var result = await _statProvider.CreateSkillAsync(statName, attribName);
            if (result == null) return StatisticResult.StatisticCreationFailed();
            return StatisticResult.StatisticCreatedSuccessfully();
        }

        /// <summary>
        /// Gets the caller's active character and returns the result.
        /// </summary> 
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <returns>A new CharacterResult object.</returns>
        public async Task<IResult> ShowStatisticsAsync(ulong callerId)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            if (character.Statistics == null || character.Statistics.Count <= 0)
                return StatisticResult.StatisticNotFound();

            StringBuilder output = new StringBuilder(character.Name + "\n");
            foreach (var statMap in character.Statistics)
            {
                var stat = StatisticResult.Show(statMap);
                output.Append(stat.Message + "\n");
            }

            return GenericResult.Generic(output.ToString());
        }

        /// <summary>
        /// Used to set a character's attributes up.
        /// </summary>
        /// <param name="callerId">The user identifier of the caller.</param>
        /// <param name="values">What to set the initial attributes to.</param>
        public async Task<IResult> SetStatisticAsync(ulong callerId, string statName, int? newValue = null)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            var statistic = await _statProvider.GetStatisticAsync(statName);
            if (statistic == null) return StatisticResult.StatisticNotFound();

            try
            {
                // TODO: Find a better way to get an instance of IProgressionStrategy
                await character.ProgressStatistic(new GenericProgressionStrategy(_statProvider, _statOptions), statistic, newValue);
                await _charProvider.UpdateCharacterAsync(character);
                return StatisticResult.StatisticSetSucessfully();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
                return GenericResult.Failure(e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Used to set a character's proficiencies up.
        /// </summary>
        /// <param name="callerId">The user identifier of the caller.</param>
        /// <param name="values">What to set the initial attributes to.</param>
        public async Task<IResult> SetProficiencyAsync(ulong callerId, string statName, bool isProficient)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            var statistic = await _statProvider.GetStatisticAsync(statName);
            if (statistic == null) return StatisticResult.StatisticNotFound();

            try
            {
                // TODO: Find a better way to get an instance of IProgressionStrategy
                await character.SetProficiency(new GenericProgressionStrategy(_statProvider, _statOptions), statistic, isProficient);
                await _charProvider.UpdateCharacterAsync(character);
                return StatisticResult.StatisticSetSucessfully();
            }
            catch (System.Exception e)
            {
                return GenericResult.Failure(e.Message);
                throw e;
            }
        }
    }
}
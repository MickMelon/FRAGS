using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.DataAccess;
using Frags.Core.Game.Statistics;
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
        /// Used to set a character's attributes up.
        /// </summary>
        /// <param name="callerId">The user identifier of the caller.</param>
        /// <param name="values">What to set the initial attributes to.</param>
        public async Task<IResult> SetStatisticAsync(ulong callerId, string statName, int newValue)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            var statistic = await _statProvider.GetStatisticAsync(statName);
            if (statistic == null) return StatisticResult.StatisticNotFound();

            try
            {
                // TODO: Find a better way to get an instance of IProgressionStrategy
                await character.SetStatistic(new GenericProgressionStrategy(_statProvider, _statOptions), statistic, newValue);
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
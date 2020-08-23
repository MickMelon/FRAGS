using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common.Exceptions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Progression;
using Frags.Core.Statistics;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels;

namespace Frags.Presentation.Controllers
{
    // TODO: separate IProgressionStrategy bits out into CharacterController?
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
        /// Used to control how characters are setup and progress.
        /// </summary>
        private readonly IProgressionStrategy _strategy;

        private readonly ICampaignProvider _campProvider;

        public StatisticController(ICharacterProvider charProvider, IStatisticProvider statProvider, IProgressionStrategy strategy, ICampaignProvider campProvider)
        {
            _charProvider = charProvider;
            _statProvider = statProvider;
            _strategy = strategy;
            _campProvider = campProvider;
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
        /// <param name="channelId">The optional id of the channel the command was executed in. Used to associate the new statistic with a campaign.</param>
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
        /// Renames an already existing statistic.
        /// </summary>
        /// <param name="statName">The name of the statistic to rename.</param>
        /// <param name="newName">The new name of the statistic.</param>
        /// <returns>A result detailing if the operation was successful or why it failed.</returns>
        /// <remarks>This method will also clear its aliases.</remarks>
        public async Task<IResult> RenameStatisticAsync(string statName, string newName)
        {
            var stat = await _statProvider.GetStatisticAsync(statName);
            if (stat == null) return StatisticResult.StatisticNotFound();

            if (await _statProvider.GetStatisticAsync(newName) != null)
                return StatisticResult.NameAlreadyExists();

            stat.Name = newName;
            stat.Aliases = newName + "/";
            await _statProvider.UpdateStatisticAsync(stat);

            return StatisticResult.StatisticUpdatedSucessfully();
        }

        /// <summary>
        /// Sets the order of an already existing statistic.
        /// </summary>
        /// <param name="statName">The name of the statistic to set its order.</param>
        /// <param name="order">An integer representing where the statistic should be placed in a sorted list.</param>
        /// <returns>A result detailing if the operation was successful or why it failed.</returns>
        public async Task<IResult> OrderStatisticAsync(string statName, int order)
        {
            Statistic stat = await _statProvider.GetStatisticAsync(statName);
            if (stat == null) return StatisticResult.StatisticNotFound();

            stat.Order = order;
            await _statProvider.UpdateStatisticAsync(stat);

            return StatisticResult.StatisticUpdatedSucessfully();
        }

        /// <summary>
        /// Adds an alias to an already existing statistic.
        /// </summary>
        /// <param name="statName">The name of the statistic to add an alias to.</param>
        /// <param name="alias">The new alias to add.</param>
        /// <returns>A result detailing if the operation was successful or why it failed.</returns>
        public async Task<IResult> AddAliasAsync(string statName, string alias)
        {
            var stat = await _statProvider.GetStatisticAsync(statName);
            if (stat == null) return StatisticResult.StatisticNotFound();

            if (await _statProvider.GetStatisticAsync(alias) != null)
                return StatisticResult.NameAlreadyExists();

            stat.Aliases += alias + "/";
            await _statProvider.UpdateStatisticAsync(stat);

            return StatisticResult.StatisticUpdatedSucessfully();
        }

        

        /// <summary>
        /// Clears the aliases of an already existing statistic.
        /// </summary>
        /// <param name="statName">The name of the statistic to add an alias to.</param>
        /// <returns>A result detailing if the operation was successful or why it failed.</returns>
        public async Task<IResult> ClearAliasesAsync(string statName)
        {
            var stat = await _statProvider.GetStatisticAsync(statName);
            if (stat == null) return StatisticResult.StatisticNotFound();

            stat.Aliases = stat.Name + "/";
            await _statProvider.UpdateStatisticAsync(stat);
            
            return StatisticResult.StatisticUpdatedSucessfully();
        }

        /// <summary>
        /// Deletes a statistic in the database.
        /// </summary>
        /// <param name="statName">The name for the new skill.</param>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> DeleteStatisticAsync(string statName)
        {
            var statistic = await _statProvider.GetStatisticAsync(statName);
            if (statistic == null)
                return StatisticResult.StatisticNotFound();

            await _statProvider.DeleteStatisticAsync(statistic);
            return StatisticResult.StatisticDeletedSuccessfully();
        }
    }
}
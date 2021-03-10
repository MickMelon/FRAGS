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
    public class StatisticCharacterController
    {
        /// <summary>
        /// Used to interact with the character database.
        /// </summary>
        private readonly ICharacterProvider _charProvider;

        /// <summary>
        /// Used to interact with the statistic database.
        /// </summary>
        private readonly IStatisticProvider _statProvider;

        private readonly ICampaignProvider _campProvider;

        /// <summary>
        /// Used to control how characters are setup and progress.
        /// </summary>
        private readonly IProgressionStrategy _fallbackStrategy;

        public StatisticCharacterController(ICharacterProvider charProvider, IStatisticProvider statProvider, IProgressionStrategy strategy, ICampaignProvider campProvider)
        {
            _charProvider = charProvider;
            _statProvider = statProvider;
            _fallbackStrategy = strategy;
            _campProvider = campProvider;
        }

        /// <summary>
        /// Gets the character associated with the id and checks
        /// if their specified statistic is higher than the given value.
        /// </summary>
        /// <param name="id">The id of the character to get.</param>
        /// <param name="statName">The name of the statistic to get.</param>
        /// <param name="minimum">Checks if the character's StatisticValue is greater than or equal to this value.</param>
        /// <returns>
        /// A result detailing if the operation was successful or why it failed.
        /// </returns>
        public async Task<IResult> CheckStatisticAsync(ulong id, string statName)
        {
            var character = await _charProvider.GetActiveCharacterAsync(id);
            if (character == null) return CharacterResult.CharacterNotFound();

            Statistic statistic = await _statProvider.GetStatisticAsync(statName, character.Campaign);
            if (statistic == null) return StatisticResult.StatisticNotFound();

            var statValue = character.GetStatistic(statistic);
            if (statValue == null) return StatisticResult.StatisticNotFound();

            return StatisticResult.StatisticCheck(character.Name, statistic.Name, statValue.Value);
        }

        public async Task<IResult> AddExperienceAsync(ulong callerId, int xp)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            var strategy = await GetProgressionStrategy(character);

            await strategy.AddExperience(character, xp);
            await _charProvider.UpdateCharacterAsync(character);

            return CharacterResult.CharacterUpdatedSuccessfully();
        }

        public async Task<IResult> AddAttributePointsAsync(ulong callerId, int pts)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            character.AttributePoints += pts;
            await _charProvider.UpdateCharacterAsync(character);

            return CharacterResult.CharacterUpdatedSuccessfully();
        }

        public async Task<IResult> AddSkillPointsAsync(ulong callerId, int pts)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            character.SkillPoints += pts;
            await _charProvider.UpdateCharacterAsync(character);

            return CharacterResult.CharacterUpdatedSuccessfully();
        }

        public async Task<IResult> ResetStatisticsAsync(ulong id)
        {
            var character = await _charProvider.GetActiveCharacterAsync(id);
            if (character == null) return CharacterResult.CharacterNotFound();

            bool result = await _fallbackStrategy.ResetCharacter(character);
            if (!result) return CharacterResult.LevelTooLow();
            await _charProvider.UpdateCharacterAsync(character);

            return StatisticResult.Reset();
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
                return StatisticResult.CharacterStatisticsNotSet();

            var strategy = await GetProgressionStrategy(character);

            return StatisticResult.ShowCharacter(character, await strategy.GetCharacterStatisticsInfo(character));
        }

        /// <summary>
        /// Used to set a character's attributes up.
        /// </summary>
        /// <param name="callerId">The user identifier of the caller.</param>
        /// <param name="values">What to set the initial attributes to.</param>
        public async Task<IResult> SetStatisticAsync(ulong callerId, string statName, int? newValue = null, bool force = false)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            Statistic statistic = await _statProvider.GetStatisticAsync(statName, character.Campaign);
            if (statistic == null) return StatisticResult.StatisticNotFound();

            var strategy = await GetProgressionStrategy(character);

            try
            {
                if (force)
                {
                    var statValue = character.GetStatistic(statistic);

                    if (newValue.HasValue)
                        statValue.Value = newValue.Value;
                }
                else
                {
                    await strategy.SetStatistic(character, statistic, newValue);
                }

                await _charProvider.UpdateCharacterAsync(character);
                return StatisticResult.StatisticSetSucessfully();
            }
            catch (System.Exception e)
            {
                if (!(e is ProgressionException))
                    System.Console.WriteLine(e);

                return GenericResult.Failure(e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Used to set a character's attributes up.
        /// </summary>
        /// <param name="callerId">The user identifier of the caller.</param>
        /// <param name="values">What to set the initial attributes to.</param>
        public async Task<IResult> UsePointsOnStatisticAsync(ulong callerId, string statName, int? newValue = null)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            Statistic statistic = await _statProvider.GetStatisticAsync(statName, character.Campaign);
            if (statistic == null) return StatisticResult.StatisticNotFound();

            try
            {
                var currentVal = character.GetStatistic(statistic).Value;

                var strategy = await GetProgressionStrategy(character);
                await strategy.SetStatistic(character, statistic, newValue + currentVal);

                await _charProvider.UpdateCharacterAsync(character);
                return StatisticResult.StatisticSetSucessfully();
            }
            catch (System.Exception e)
            {
                if (!(e is ProgressionException))
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
        public async Task<IResult> SetProficiencyAsync(ulong callerId, string statName, bool isProficient, bool force = false)
        {
            var character = await _charProvider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            Statistic statistic = await _statProvider.GetStatisticAsync(statName, character.Campaign);
            if (statistic == null) return StatisticResult.StatisticNotFound();

            try
            {
                if (force)
                {
                    var statVal = character.GetStatistic(statistic);
                    statVal.IsProficient = isProficient;
                }
                else
                {
                    var strategy = await GetProgressionStrategy(character);
                    await strategy.SetProficiency(character, statistic, isProficient);
                }
                
                await _charProvider.UpdateCharacterAsync(character);
                return StatisticResult.StatisticSetSucessfully();
            }
            catch (System.Exception e)
            {
                return GenericResult.Failure(e.Message);
                throw e;
            }
        }

        private async Task<IProgressionStrategy> GetProgressionStrategy(Character character)
        {
            if (character.Campaign != null)
                return await _campProvider.GetProgressionStrategy(character.Campaign);
            
            return _fallbackStrategy;
        }
    }
}
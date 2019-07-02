using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Frags.Presentation.Results;
using Microsoft.Extensions.Configuration;

namespace Frags.Presentation.Controllers
{
    /// <summary>
    /// This class controls roll related actions.
    /// </summary>
    public class RollController
    {
        /// <summary>
        /// Used to interact with the character database.
        /// </summary>
        private readonly ICharacterProvider _provider;

        /// <summary>
        /// Used to interact with the statistic database.
        /// </summary>
        private readonly IStatisticProvider _statProvider;

        /// <summary>
        /// Used when a character does not belong to a Campaign.
        /// </summary>
        private readonly IRollStrategy _defaultStrategy;

        /// <summary>
        /// Used when a character is in a Campaign.
        /// </summary>
        private readonly List<IRollStrategy> _strategies;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="RollController" /> class.
        /// </summary>
        /// <param name="provider">The CharacterProvider.</param>
        public RollController(ICharacterProvider provider, IStatisticProvider statProvider, IRollStrategy defaultStrategy, List<IRollStrategy> strategies)
        {
            _provider = provider;
            _statProvider = statProvider;

            _defaultStrategy = defaultStrategy;
            _strategies = strategies;
        }

        /// <summary>
        /// Performs a roll on a character's statistic and returns the result.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="statName">The statistic name.</param>
        /// <returns>The result of the roll.</returns>
        public async Task<IResult> RollStatisticAsync(ulong callerId, string statName, bool useEffects = false)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            Statistic stat;
            IRollStrategy strategy;

            if (character.Campaign == null)
            {
                stat = await _statProvider.GetStatisticAsync(statName);
                strategy = _defaultStrategy;
            }
            else
            {
                stat = await _statProvider.GetStatisticFromCampaignAsync(statName, character.Campaign.Id);
                strategy = GetCampaignStrategy(character.Campaign);
            }

            if (stat == null) return StatisticResult.StatisticNotFound();

            string result = strategy.GetRollMessage(stat, character, useEffects);

            if (!string.IsNullOrEmpty(result))
                return RollResult.Roll(result);

            return RollResult.RollFailed();
        }

        /// <summary>
        /// Performs a roll on a character's statistic assuming it is set to the value given and returns the result.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="statName">The statistic name.</param>
        /// <param name="newValue">The new, temporary value of the character's statistic.</param>
        /// <returns>The result of the roll.</returns>
        public async Task<IResult> RollStatisticWithValueAsync(ulong callerId, string statName, int newValue, string displayName = null)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            Statistic stat;
            IRollStrategy strategy;

            if (character.Campaign == null)
            {
                stat = await _statProvider.GetStatisticAsync(statName);
                strategy = _defaultStrategy;
            }
            else
            {
                stat = await _statProvider.GetStatisticFromCampaignAsync(statName, character.Campaign.Id);
                strategy = GetCampaignStrategy(character.Campaign);
            }

            if (displayName != null)
                character.Name = displayName;

            var current = character.GetStatistic(stat);

            if (current == null)
                character.SetStatistic(stat, new StatisticValue(newValue));
            else
                current.Value = newValue;

            string msg = strategy.GetRollMessage(stat, character);

            if (!string.IsNullOrEmpty(msg))
                return RollResult.Roll(msg);

            return RollResult.RollFailed();
        }

        /// <summary>
        /// Performs a duel between two characters rolling a statistic and returns the result.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="targetId">Discord ID of the target.</param>
        /// <param name="statName">The statistic name.</param>
        /// <returns>The result of the roll.</returns>
        public async Task<IResult> RollStatisticAgainstAsync(ulong callerId, ulong targetId, string statName, bool useEffects = false)
        {
            var caller = await _provider.GetActiveCharacterAsync(callerId);
            if (caller == null) return CharacterResult.CharacterNotFound();

            var target = await _provider.GetActiveCharacterAsync(targetId);
            if (target == null) return CharacterResult.CharacterNotFound();

            Statistic stat;
            IRollStrategy strategy;

            if (caller.Campaign == null)
            {
                stat = await _statProvider.GetStatisticAsync(statName);
                strategy = _defaultStrategy;
            }
            else
            {
                stat = await _statProvider.GetStatisticFromCampaignAsync(statName, caller.Campaign.Id);
                strategy = GetCampaignStrategy(caller.Campaign);
            }

            double? callerRoll = strategy.RollStatistic(stat, caller, useEffects);
            double? targetRoll = strategy.RollStatistic(stat, target, useEffects);

            if (callerRoll.HasValue && targetRoll.HasValue)
                return RollResult.RollAgainst(caller.Name, target.Name, callerRoll.Value, targetRoll.Value);

            return RollResult.RollFailed();
        }

        private IRollStrategy GetCampaignStrategy(Campaign campaign) =>
            _strategies.Find(x => x.GetType().Name.ContainsIgnoreCase(campaign.RollOptions.RollStrategy));
    }
}
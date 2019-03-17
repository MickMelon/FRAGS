using System.Threading.Tasks;
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
        /// Used to determine which RollStrategy to use.
        /// </summary>
        private readonly IRollStrategy _strategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollController" /> class.
        /// </summary>
        /// <param name="provider">The CharacterProvider.</param>
        public RollController(ICharacterProvider provider, IStatisticProvider statProvider, RollOptions options)
        {
            _provider = provider;
            _statProvider = statProvider;

            _strategy = GetStrategy(options.RollMode);
        }

        /// <summary>
        /// Used to get an instance of IRollStrategy by reading
        /// from the Configuartion passed to this instance of RollController.
        /// </summary>
        /// <remarks>
        /// There's definitely a better way to implement this, namely
        /// with a separate factory class and interfaces to make it
        /// dependency injection friendly, just thought it was overkill.
        private IRollStrategy GetStrategy(RollMode mode)
        {
            switch (mode)
            {
                case RollMode.Mock: return new MockRollStrategy();
                case RollMode.Frags: return new FragsRollStrategy();
                default: return null;
            }
        }

        /// <summary>
        /// Performs a roll on a character's statistic and returns the result.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="statName">The statistic name.</param>
        /// <returns>The result of the roll.</returns>
        public async Task<IResult> RollStatisticAsync(ulong callerId, string statName)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            var stat = await _statProvider.GetStatisticAsync(statName);
            if (stat == null) return StatisticResult.StatisticNotFound();

            string result = _strategy.GetRollMessage(stat, character);

            if (!string.IsNullOrEmpty(result))
                return RollResult.Roll(result);

            return RollResult.RollFailed();
        }

        /// <summary>
        /// Performs a duel between two characters rolling a statistic and returns the result.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="targetId">Discord ID of the target.</param>
        /// <param name="statName">The statistic name.</param>
        /// <returns>The result of the roll.</returns>
        public async Task<IResult> RollStatisticAgainstAsync(ulong callerId, ulong targetId, string statName)
        {
            var caller = await _provider.GetActiveCharacterAsync(callerId);
            if (caller == null) return CharacterResult.CharacterNotFound();

            var target = await _provider.GetActiveCharacterAsync(targetId);
            if (target == null) return CharacterResult.CharacterNotFound();

            var stat = await _statProvider.GetStatisticAsync(statName);
            if (stat == null) return StatisticResult.StatisticNotFound();

            double? callerRoll = caller.RollStatistic( _strategy, stat);
            double? targetRoll = target.RollStatistic( _strategy, stat);

            if (callerRoll.HasValue && targetRoll.HasValue)
                return RollResult.RollAgainst(caller.Name, target.Name, callerRoll.Value, targetRoll.Value);

            return RollResult.RollFailed();
        }
    }
}
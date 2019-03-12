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
        /// Used to determine which RollStrategy to use.
        /// </summary>
        private readonly IRollStrategy _strategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollController" /> class.
        /// </summary>
        /// <param name="provider">The CharacterProvider.</param>
        public RollController(ICharacterProvider provider, RollOptions options)
        {
            _provider = provider;

            _strategy = GetStrategy(options.RollMode);
        }

        /// <summary>
        /// Used to get an instance of IRollStrategy by reading
        /// from the Configuartion passed to this instance of RollController.
        /// </summary>
        private IRollStrategy GetStrategy(RollMode mode)
        {
            switch (mode)
            {
                case RollMode.Mock: return new MockRollStrategy();
                default: return null;
            }
        }

        /// <summary>
        /// Performs a roll on a character's skill and returns the result.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="skill">The skill name.</param>
        /// <returns>The result of the roll.</returns>
        public async Task<IResult> RollAsync(ulong callerId, string skill)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            // Check valid skill name
            // dummy for testing
            if (skill.EqualsIgnoreCase("invalid")) 
                return SkillResult.SkillNotFound();

            int roll = character.Roll(skill);

            // We need to resolve the string into a Statistic first
            // and use Configuration to figure out which Strategy to use
            double? result = character.RollStatistic(new Skill(), _strategy);

            return RollResult.Roll(character.Name, skill, roll);
        }

        /// <summary>
        /// Performs a duel between two characters rolling a skill and returns the result.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="targetId">Discord ID of the target.</param>
        /// <param name="skill">The skill name.</param>
        /// <returns>The result of the roll.</returns>
        public async Task<IResult> RollAgainstAsync(ulong callerId, ulong targetId, string skill)
        {
            var caller = await _provider.GetActiveCharacterAsync(callerId);
            if (caller == null) return CharacterResult.CharacterNotFound();

            var target = await _provider.GetActiveCharacterAsync(targetId);
            if (target == null) return CharacterResult.CharacterNotFound();

            // dummy for testing
            if (skill.EqualsIgnoreCase("invalid")) 
                return SkillResult.SkillNotFound();

            int callerRoll = caller.Roll(skill) + 1;
            int targetRoll = target.Roll(skill);

            return RollResult.RollAgainst(caller.Name, target.Name, callerRoll, targetRoll);
        }
    }
}
using System.Threading.Tasks;
using Frags.Core.Controllers.Results;
using Frags.Core.DataAccess;

namespace Frags.Core.Controllers
{
    public class RollController
    {
        private readonly ICharacterProvider _provider;

        public RollController(ICharacterProvider provider)
        {
            _provider = provider;
        }

        public async Task<IResult> RollAsync(ulong callerId, string skill)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            // Check valid skill name

            int roll = character.Roll(skill);
            return RollResult.Roll(character.Name, skill, roll);
        }

        public async Task<IResult> RollAgainstAsync(ulong callerId, ulong targetId, string skill)
        {
            var caller = await _provider.GetActiveCharacterAsync(callerId);
            if (caller == null) return CharacterResult.CharacterNotFound();

            var target = await _provider.GetActiveCharacterAsync(targetId);
            if (target == null) return CharacterResult.CharacterNotFound();

            int callerRoll = caller.Roll(skill) + 1;
            int targetRoll = target.Roll(skill);

            return RollResult.RollAgainst(caller.Name, target.Name, callerRoll, targetRoll);
        }
    }
}
using System.Threading.Tasks;
using Frags.Core.Controllers.Results;
using Frags.Core.DataAccess;

namespace Frags.Core.Controllers
{
    public class CharacterController
    {
        private readonly ICharacterProvider _provider;

        public CharacterController(ICharacterProvider provider)
        {
            _provider = provider;
        }

        public async Task<IResult> ShowCharacterAsync(ulong callerId)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            return GenericResult.Result($"{character.Name}: {character.Id}");
        }

        public async Task<IResult> RollAsync(ulong callerId, string skill)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();

            // Check valid skill name

            int roll = character.Roll(skill);
            return GenericResult.Result($"{character.Name} rolled a {roll} in {skill}.");
        }

        public async Task<IResult> RollAgainstAsync(ulong callerId, ulong targetId, string skill)
        {
            var caller = await _provider.GetActiveCharacterAsync(callerId);
            if (caller == null) return CharacterResult.CharacterNotFound();

            var target = await _provider.GetActiveCharacterAsync(targetId);
            if (target == null) return CharacterResult.CharacterNotFound();

            int callerRoll = caller.Roll(skill) + 1;
            int targetRoll = target.Roll(skill);

            if (callerRoll > targetRoll)
                return GenericResult.Result($"{caller.Name} rolled {callerRoll} beating {target.Name}'s {targetRoll}!");
            
            return GenericResult.Result($"{caller.Name} rolled {callerRoll} but failed to beat {target.Name}'s {targetRoll}");
        }
    }
}
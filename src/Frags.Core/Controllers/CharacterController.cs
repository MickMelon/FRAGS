using System.Threading.Tasks;
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

        public async Task<string> ShowCharacterAsync(ulong callerId)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return "Invalid character";

            return $"{character.Name}: {character.Id}";
        }

        public async Task<string> RollAsync(ulong callerId, string skill)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return "Invalid character";

            // Check valid skill name

            int roll = character.Roll(skill);
            return $"{character.Name} rolled a {roll} in {skill}.";
        }

        public async Task<string> RollAgainstAsync(ulong callerId, ulong targetId, string skill)
        {
            var caller = await _provider.GetActiveCharacterAsync(callerId);
            if (caller == null) return "You do not have an active character.";

            var target = await _provider.GetActiveCharacterAsync(targetId);
            if (target == null) return "They do not have an active character.";

            int callerRoll = caller.Roll(skill) + 1;
            int targetRoll = target.Roll(skill);

            if (callerRoll > targetRoll)
                return $"{caller.Name} rolled {callerRoll} beating {target.Name}'s {targetRoll}!";
            
            return $"{caller.Name} rolled {callerRoll} but failed to beat {target.Name}'s {targetRoll}";
        }
    }
}
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

        public async Task<string> ShowCharacterAsync(ulong discordId)
        {
            var character = await _provider.GetActiveCharacterAsync(discordId);
            if (character == null) return "Invalid character";

            return $"{character.Name}: {character.Id}";
        }
    }
}
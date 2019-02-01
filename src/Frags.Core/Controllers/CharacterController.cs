using System.Threading.Tasks;
using Frags.Core.Controllers.Results;
using Frags.Core.Controllers.ViewModels;
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
            return CharacterResult.Show(character);
        }
    }
}
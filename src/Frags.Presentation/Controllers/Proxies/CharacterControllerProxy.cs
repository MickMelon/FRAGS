using System.Threading.Tasks;
using Frags.Core.DataAccess;
using Frags.Presentation.Controllers.Interfaces;
using Frags.Presentation.Results;

namespace Frags.Presentation.Controllers.Proxies
{
    public class CharacterControllerProxy : ICharacterController
    {
        /// <summary>
        /// Used to interact with the character database.
        /// </summary>
        private readonly ICharacterProvider _provider;

        private readonly CharacterController _realController;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterController" /> class.
        /// </summary>
        /// <param name="provider">The CharacterProvider.</param>
        public CharacterControllerProxy(ICharacterProvider provider)
        {
            _provider = provider;
        }

        public async Task<IResult> ShowCharacterAsync(ulong callerId)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();
            return _realController.ShowCharacter(character);
        }


    }
}
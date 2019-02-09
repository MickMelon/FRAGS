using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Presentation.Results;

namespace Frags.Presentation.Controllers
{
    /// <summary>
    /// This class controls character related actions.
    /// </summary>
    public class CharacterController
    {
        /// <summary>
        /// Used to interact with the character database.
        /// </summary>
        private readonly ICharacterProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterController" /> class.
        /// </summary>
        /// <param name="provider">The CharacterProvider.</param>
        public CharacterController(ICharacterProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Gets the caller's active character and returns the result.
        /// </summary> 
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <returns>A new CharacterResult object.</returns>
        public async Task<IResult> ShowCharacterAsync(ulong callerId)
        {
            var character = await _provider.GetActiveCharacterAsync(callerId);
            if (character == null) return CharacterResult.CharacterNotFound();
            return CharacterResult.Show(character);
        }

        /// <summary>
        /// Creates a new character.
        /// </summary>
        /// <param name="callerId">Discord ID of the caller.</param>
        /// <param name="name">The desired character name.</param>
        /// <returns>A new CharacterResult object.</returns>
        public async Task<IResult> CreateCharacterAsync(ulong callerId, string name)
        {
            var characters = await _provider.GetAllCharactersAsync(callerId);
            var existing = characters.Where(c => c.Name.EqualsIgnoreCase(name)).FirstOrDefault();
            if (existing != null) return CharacterResult.NameAlreadyExists();

            var newCharacter = await _provider.CreateCharacterAsync(callerId, name);
            return CharacterResult.CharacterCreatedSuccessfully();
        }
    }
}
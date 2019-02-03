using System.Threading.Tasks;
using Frags.Core.Characters;
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

        public IResult ShowCharacter(Character character)
        {
            return CharacterResult.Show(character);
        }
    }
}
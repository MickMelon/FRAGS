
using Frags.Core.Common;
using Frags.Core.Models.Characters;
using Frags.Presentation.ViewModels;

namespace Frags.Presentation.Results
{
    /// <summary>
    /// Represents a result type for characters.
    /// </summary>
    public class CharacterResult : BaseResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterResult" /> class.
        /// </summary>
        public CharacterResult(string message, bool success = true, object viewModel = null) : base(message, success, viewModel)
        {
        }

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        /// <param name="character">The character to show.</param>
        public static IResult Show(Character character) =>
            new CharacterResult($"{character.Name}: {character.Id}", 
                viewModel: new ShowCharacterViewModel()
                {
                    Name = character.Name,
                    Story = character.Story,
                    Description = character.Description
                });        

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static CharacterResult CharacterNotFound() =>
            new CharacterResult(Messages.CHAR_NOT_FOUND, false);

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static CharacterResult NpcNotFound() =>
            new CharacterResult(Messages.NPC_NOT_FOUND, false);
    }
}
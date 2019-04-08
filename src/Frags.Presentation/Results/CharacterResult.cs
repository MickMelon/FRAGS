using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Presentation.ViewModels.Characters;

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
        public static IResult Show(Character character, int level) =>
            new CharacterResult($"{character.Name}: {character.Id}", 
                viewModel: new ShowCharacterViewModel()
                {
                    Name = character.Name,
                    Story = character.Story,
                    Description = character.Description,
                    Level = level,
                    Experience = character.Experience
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

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static CharacterResult NameAlreadyExists() =>
            new CharacterResult(Messages.CHAR_NAME_EXISTS, false);

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static CharacterResult CharacterCreatedSuccessfully() =>
            new CharacterResult(Messages.CHAR_CREATE_SUCCESS, true);

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static CharacterResult CharacterActive() =>
            new CharacterResult(Messages.CHAR_ACTIVE, true);

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static CharacterResult CharacterAlreadyActive() =>
            new CharacterResult(Messages.CHAR_ALREADY_ACTIVE, false);

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static CharacterResult LevelTooLow() =>
            new CharacterResult(Messages.CHAR_LEVEL_TOO_LOW, false);

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static CharacterResult LevelTooHigh() =>
            new CharacterResult(Messages.CHAR_LEVEL_TOO_HIGH, false);
    }
}
using System;
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
        public static IResult Show(Character character, int level, string progressionInfo) =>
            new CharacterResult($"{character.Name}: {character.Id}", 
                viewModel: new ShowCharacterViewModel()
                {
                    Name = character.Name,
                    Story = character.Story,
                    Description = character.Description,
                    Level = level,
                    Money = character.Money,
                    Experience = character.Experience,
                    ProgressionInformation = progressionInfo
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
        public static CharacterResult CharacterActive(string charName) =>
            new CharacterResult(string.Format(Messages.CHAR_ACTIVE, charName), true);

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

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static CharacterResult CharacterUpdatedSuccessfully() =>
            new CharacterResult(Messages.CHAR_UPDATE_SUCCESS, true);

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static CharacterResult MoneyGiven(int amount, string recipient) =>
            new CharacterResult(Messages.CHAR_UPDATE_SUCCESS, true);

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static CharacterResult TooManyCharacters() =>
            new CharacterResult(Messages.CHAR_TOO_MANY, false);
    }
}
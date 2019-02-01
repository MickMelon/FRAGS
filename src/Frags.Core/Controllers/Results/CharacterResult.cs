using Frags.Core.Common;
using Frags.Core.Controllers.ViewModels;
using Frags.Core.Models.Characters;

namespace Frags.Core.Controllers.Results
{
    public class CharacterResult : BaseResult
    {
        public CharacterResult(string message, bool success = true) : base(message, success)
        {
        }

        public static IResult Show(Character character) =>
            new CharacterResult($"{character.Name}: {character.Id}")
                .WithViewModel(new ShowCharacterViewModel()
                {
                    Name = character.Name,
                    Story = character.Story,
                    Description = character.Description
                });            

        public static CharacterResult CharacterNotFound() =>
            new CharacterResult(Messages.CHAR_NOT_FOUND, false);

        public static CharacterResult NpcNotFound() =>
            new CharacterResult(Messages.NPC_NOT_FOUND, false);
    }
}
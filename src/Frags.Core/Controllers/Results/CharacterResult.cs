using Frags.Core.Common;

namespace Frags.Core.Controllers.Results
{
    public class CharacterResult : BaseResult
    {
        public CharacterResult(string message) : base(message)
        {
        }

        public static CharacterResult CharacterNotFound() =>
            new CharacterResult(Messages.CHAR_NOT_FOUND);

        public static CharacterResult NpcNotFound() =>
            new CharacterResult(Messages.NPC_NOT_FOUND);
    }
}
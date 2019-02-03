using Frags.Core.Common;

namespace Frags.Presentation.Results
{
    public class SkillResult : BaseResult
    {
        public SkillResult(string message, bool success = true, object viewModel = null) : base(message, success, viewModel)
        {
        }

        public static SkillResult SkillNotFound() =>
            new SkillResult(Messages.SKILL_NOT_FOUND, false);
    }
}
namespace Frags.Core.Controllers.Results
{
    public class SkillResult : BaseResult
    {
        public SkillResult(string message, bool success = true, object viewModel = null) : base(message, success, viewModel)
        {
        }

        public static SkillResult SkillNotFound() =>
            new SkillResult("Skill not found", false);
    }
}
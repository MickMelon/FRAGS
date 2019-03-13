using Frags.Core.Common;

namespace Frags.Presentation.Results
{
    public class StatisticResult : BaseResult
    {
        public StatisticResult(string message, bool success = true, object viewModel = null) : base(message, success, viewModel)
        {
        }

        public static StatisticResult StatisticNotFound() =>
            new StatisticResult(Messages.STAT_NOT_FOUND, false);
    }
}
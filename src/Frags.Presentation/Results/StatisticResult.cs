using Frags.Core.Common;

namespace Frags.Presentation.Results
{
    public class StatisticResult : BaseResult
    {
        public StatisticResult(string message, bool success = true, object viewModel = null) : base(message, success, viewModel)
        {
        }

        public static StatisticResult StatisticSetSucessfully() =>
            new StatisticResult(Messages.STAT_SET_SUCCESS, false);

        public static StatisticResult StatisticNotFound() =>
            new StatisticResult(Messages.STAT_NOT_FOUND, false);

        public static StatisticResult TooManyAtMax(int limit) =>
            new StatisticResult(string.Format(Messages.STAT_TOO_MANY_AT_MAX, limit), false);
    }
}
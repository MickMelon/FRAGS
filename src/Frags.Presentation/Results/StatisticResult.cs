using System;
using System.Collections.Generic;
using System.Linq;
using Frags.Core.Common;
using Frags.Core.Statistics;
using Frags.Presentation.ViewModels;

namespace Frags.Presentation.Results
{
    public class StatisticResult : BaseResult
    {
        public StatisticResult(string message, bool success = true, object viewModel = null) : base(message, success, viewModel)
        {
        }

        /// <summary>
        /// Returns a new <see cref="CharacterResult" />.
        /// </summary>
        public static StatisticResult NameAlreadyExists() =>
            new StatisticResult(Messages.STAT_NAME_EXISTS, false);

        public static StatisticResult StatisticCreatedSuccessfully() =>
            new StatisticResult(Messages.STAT_CREATE_SUCCESS, true);

        public static StatisticResult StatisticCreationFailed() =>
            new StatisticResult(Messages.STAT_CREATE_FAILURE, false);

        public static StatisticResult StatisticSetSucessfully() =>
            new StatisticResult(Messages.STAT_SET_SUCCESS, true);

        public static StatisticResult StatisticNotFound() =>
            new StatisticResult(Messages.STAT_NOT_FOUND, false);

        public static StatisticResult TooManyAtMax(int limit) =>
            new StatisticResult(string.Format(Messages.STAT_TOO_MANY_AT_MAX, limit), false);

        public static IResult Show(StatisticMapping statMap)
        {
            var stat = new ShowStatisticViewModel()
            {
                Name = statMap.Statistic.Name,
                Description = statMap.Statistic.Description,
                Value = statMap.StatisticValue.Value,
                IsProficient = statMap.StatisticValue.IsProficient,
                Proficiency = statMap.StatisticValue.Proficiency
            };

            var message = $"**{stat.Name}:** {stat.Value}";
            if (stat.IsProficient)
                message += "*";

            return new StatisticResult(message,
                viewModel: stat);
        }
    }
}
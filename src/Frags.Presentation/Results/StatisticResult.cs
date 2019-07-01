using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Statistics;
using Frags.Presentation.ViewModels.Statistics;
using Attribute = Frags.Core.Statistics.Attribute;

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

        public static StatisticResult StatisticUpdatedSucessfully() =>
            new StatisticResult(Messages.STAT_UPDATE_SUCCESS, true);

        public static StatisticResult StatisticDeletedSuccessfully() =>
            new StatisticResult(Messages.STAT_DELETE_SUCCESS, true);

        public static StatisticResult StatisticCreationFailed() =>
            new StatisticResult(Messages.STAT_CREATE_FAILURE, false);

        public static StatisticResult StatisticSetSucessfully() =>
            new StatisticResult(Messages.STAT_SET_SUCCESS, true);

        public static StatisticResult StatisticNotFound() =>
            new StatisticResult(Messages.STAT_NOT_FOUND, false);

        public static StatisticResult StatisticCheck(string charName, string statName, int actual) =>
            new StatisticResult(string.Format(Messages.STAT_CHECK, charName, statName, actual), true);

        public static StatisticResult Reset() =>
            new StatisticResult(Messages.STAT_RESET);

        public static StatisticResult TooManyAtMax(int limit) =>
            new StatisticResult(string.Format(Messages.STAT_TOO_MANY_AT_MAX, limit), false);

        public static IResult Show(StatisticMapping statMap)
        {
            ShowStatisticViewModel stat = null;

            if (statMap.Statistic is Attribute attrib)
            {
                stat = new ShowAttributeViewModel(attrib.Name, attrib.Description, attrib.AliasesArray, attrib.Id, statMap?.StatisticValue.Value,
                    statMap?.StatisticValue.IsProficient, statMap?.StatisticValue.Proficiency);
            }
            else if (statMap.Statistic is Skill s)
            {
                var attribViewModel = new ShowAttributeViewModel(s.Attribute.Name, s.Attribute.Description, s.Attribute.AliasesArray, s.Attribute.Id, null, null, null);

                stat = new ShowSkillViewModel(s.Name, s.Description, s.AliasesArray, s.Id, statMap?.StatisticValue.Value,
                    statMap?.StatisticValue.IsProficient, statMap?.StatisticValue.Proficiency, s.MinimumValue, attribViewModel);
            }
            else
            {
                return StatisticResult.StatisticNotFound();
            }

            var message = $"**{stat.Name}:** {stat.Value?.ToString() ?? "N/A"}";
            if (stat.IsProficient.HasValue && stat.IsProficient.Value)
                message += "*";

            return new StatisticResult(message,
                viewModel: stat);
        }

        public static IResult ShowCharacter(Character character, string progressionInfo)
        {
            var result = new ShowCharacterStatisticsViewModel
            {
                CharacterName = character.Name,
                AttributePoints = character.AttributePoints,
                SkillPoints = character.SkillPoints
            };

            // Get a list of all statistic view models using Show()
            var stats = new List<ShowStatisticViewModel>();
            foreach (var statMap in character.Statistics)
            {
                var viewModel = (ShowStatisticViewModel)Show(statMap).ViewModel;

                if (viewModel != null)
                    stats.Add(viewModel);
            }

            // Get a list of skill view models associated with each attribute
            foreach (var attribute in stats.OfType<ShowAttributeViewModel>())
            {
                var skills = stats.OfType<ShowSkillViewModel>().Where(x => x.Attribute.Name.Equals(attribute.Name)).ToList();
                result.Statistics.Add(attribute, skills);
            }

            result.ProgressionInformation = progressionInfo;

            return new StatisticResult($"{character.Statistics.Count()} Statistics found",
                viewModel: result);
        }
    }
}
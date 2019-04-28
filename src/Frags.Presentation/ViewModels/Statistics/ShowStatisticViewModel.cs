using Frags.Presentation.Attributes;

namespace Frags.Presentation.ViewModels.Statistics
{
    /// <summary>
    /// The ViewModel used for showing a statistic.
    /// </summary>
    [ViewModel]
    public abstract class ShowStatisticViewModel
    {
        public ShowStatisticViewModel(string name, string desc, string[] aliases, int order, int? value, bool? isProf, double? prof)
        {
            Name = name;
            Description = desc;
            Aliases = aliases;
            Order = order;
            Value = value;
            IsProficient = isProf;
            Proficiency = prof;
        }

        public ShowStatisticViewModel() { }

        /// <summary>
        /// The statistic's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The statistic's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The statistic's aliases.
        /// </summary>
        public string[] Aliases { get; set; }

        /// <summary>
        /// The statistic's current value.
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// The statistic's proficient status.
        /// </summary>
        public bool? IsProficient { get; set; }

        /// <summary>
        /// The statistic's proficiency bonus.
        /// </summary>
        public double? Proficiency { get; set; }

        public int Order { get; set; }
    }
}
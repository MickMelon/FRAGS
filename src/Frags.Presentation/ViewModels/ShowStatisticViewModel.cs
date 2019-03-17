using Frags.Presentation.Attributes;

namespace Frags.Presentation.ViewModels
{
    /// <summary>
    /// The ViewModel used for showing a statistic.
    /// </summary>
    [ViewModel]
    public class ShowStatisticViewModel
    {
        /// <summary>
        /// The statistic's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The statistic's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The statistic's current value.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// The statistic's proficient status.
        /// </summary>
        public bool IsProficient { get; set; }

        /// <summary>
        /// The statistic's proficiency bonus.
        /// </summary>
        public double Proficiency { get; set; }
    }
}
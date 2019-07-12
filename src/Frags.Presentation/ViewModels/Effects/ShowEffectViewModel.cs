using System.Collections.Generic;
using Frags.Core.Statistics;
using Frags.Presentation.Attributes;

namespace Frags.Presentation.ViewModels.Effects
{
    /// <summary>
    /// The ViewModel used for showing an Effect.
    /// </summary>
    [ViewModel]
    public class ShowEffectViewModel
    {
        public ShowEffectViewModel(string name, string desc, Dictionary<Statistic, StatisticValue> statisticEffects)
        {
            Name = name;
            Description = desc;
            StatisticEffects = statisticEffects;
        }

        public ShowEffectViewModel() { }

        /// <summary>
        /// The Effect's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Effect's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Effect's statistic effects.
        /// </summary>
        public Dictionary<Statistic, StatisticValue> StatisticEffects { get; set; }
    }
}
using Frags.Presentation.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frags.Presentation.ViewModels.Statistics
{
    [ViewModel]
    public class ShowCharacterStatisticsViewModel
    {
        public IDictionary<ShowAttributeViewModel, ICollection<ShowSkillViewModel>> Statistics { get; set; } =
            new Dictionary<ShowAttributeViewModel, ICollection<ShowSkillViewModel>>();

        public string CharacterName { get; set; }

        public int AttributePoints { get; set; }
        public int SkillPoints { get; set; }

        /// <summary>
        /// Important info related to the character's statistics provided by the ProgressionStrategy.
        /// </summary>
        public string ProgressionInformation { get; set; }
    }
}

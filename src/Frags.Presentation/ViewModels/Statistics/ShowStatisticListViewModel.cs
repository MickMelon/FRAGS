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

        public int AttributePoints { get; set; }
        public int SkillPoints { get; set; }
    }
}

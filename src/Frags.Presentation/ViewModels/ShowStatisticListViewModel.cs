using Frags.Presentation.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frags.Presentation.ViewModels
{
    [ViewModel]
    public class ShowStatisticListViewModel
    {
        public IDictionary<ShowAttributeViewModel, ICollection<ShowSkillViewModel>> Statistics { get; set; } =
            new Dictionary<ShowAttributeViewModel, ICollection<ShowSkillViewModel>>();
    }
}

using System.Collections.Generic;
using Frags.Core.Campaigns;
using Frags.Core.Common;
using Frags.Core.Statistics;
using Frags.Presentation.Attributes;
using Frags.Presentation.ViewModels.Statistics;

namespace Frags.Presentation.ViewModels.Campaigns
{
    [ViewModel]
    public class ShowCampaignViewModel
    {
        public string Name { get; set; }

        public User Owner { get; set; }

        public List<Channel> Channels { get; set; }

        public IEnumerable<string> CharacterNames { get; set; }

        public StatisticOptions StatisticOptions { get; set; }

        public List<ShowStatisticViewModel> Statistics { get; set; }
    }
}
using System.Collections.Generic;
using Frags.Core.Statistics;
using Frags.Database.Campaigns;

namespace Frags.Database.Statistics
{
    public class StatisticOptionsDto : StatisticOptions
    {
        public int Id { get; set; }
        new public ICollection<ChannelDto> ExpEnabledChannels { get; set; }
    }
}
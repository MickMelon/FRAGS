using System.Collections.Generic;
using Frags.Core.Statistics;

namespace Frags.Database.Statistics
{
    public class StatisticOptionsDto : StatisticOptions
    {
        public int Id { get; set; }
        new public ICollection<ChannelDto> ExpEnabledChannels { get; set; }
    }

    public class ChannelDto
    {
        public ulong Id { get; set; }
    }
}
using System.Collections.Generic;

namespace Frags.Core.Statistics
{
    public interface IStatsheet
    {
        Dictionary<Statistic, StatisticValue> Statistics { get; set; }
    }
}
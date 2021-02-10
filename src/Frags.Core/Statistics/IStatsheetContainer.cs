using System.Collections.Generic;

namespace Frags.Core.Statistics
{
    public interface IStatsheetContainer
    {
        int Id { get; }
        Dictionary<Statistic, StatisticValue> Statistics { get; set; }
    }
}
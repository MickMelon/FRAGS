using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Common.Extensions;
using Frags.Core.Statistics;
using Attribute = Frags.Core.Statistics.Attribute;

namespace Frags.Core.DataAccess
{
    /**
     * The real implementation of this class would be
     * contained in the Frags.Database project.    
     */
    /// <inheritdoc/>
    public class MockStatisticProvider : IStatisticProvider
    {
        private List<Statistic> _statistics;
        private int id;

        public MockStatisticProvider()
        {
            var str = new Attribute("1", "Strength");
            var lifting = new Skill(str, "2", "Powerlifting");

            _statistics = new List<Statistic>()
            {
                str,
                lifting
            };

            id = _statistics.Count + 1;
        }

        public async Task<Attribute> CreateAttributeAsync(string name)
        {
            if (GetStatisticAsync(name) != null) throw new ArgumentException("Statistic name was not unique.");

            var stat = new Attribute(id++.ToString(), name);
            _statistics.Add(stat);
            return await Task.FromResult(stat);
        }

        public async Task<Skill> CreateSkillAsync(string name, string attribName)
        {
            if (GetStatisticAsync(name) != null) throw new ArgumentException("Statistic name was not unique.");

            var attrib = _statistics.OfType<Attribute>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(attribName));
            if (attrib == null) return null;

            var stat = new Skill(attrib, id++.ToString(), name);
            _statistics.Add(stat);

            return await Task.FromResult(stat);
        }

        public async Task<Statistic> GetStatisticAsync(string name)
        {
            return await Task.FromResult(_statistics.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name)));
        }

        public async Task UpdateStatisticAsync(Statistic statistic)
        {
            await Task.Delay(0); // to get rid of warning
            _statistics[_statistics.FindIndex(x => x.Id.Equals(statistic.Id))] = statistic;
        }
    }
}
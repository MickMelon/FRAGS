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
            var str = new Attribute("Strength");
            
            _statistics = new List<Statistic>()
            {
                str,
                new Attribute("Perception"),
                new Attribute("Endurance"),
                new Attribute("Charisma"),
                new Attribute("Intelligence"),
                new Attribute("Agility"),
                new Attribute("Luck"),
                new Skill(str, "Powerlifting")
            };

            id = _statistics.Count + 1;
        }

        public async Task<Attribute> CreateAttributeAsync(string name)
        {
            if (await GetStatisticAsync(name) != null) throw new ArgumentException("Statistic name was not unique.");

            var stat = new Attribute(id++.ToString(), name);
            _statistics.Add(stat);
            return await Task.FromResult(stat);
        }

        public async Task<Skill> CreateSkillAsync(string name, string attribName)
        {
            if (await GetStatisticAsync(name) != null) throw new ArgumentException("Statistic name was not unique.");

            var attrib = _statistics.OfType<Attribute>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(attribName));
            if (attrib == null) return null;

            var stat = new Skill(attrib, id++.ToString(), name);
            _statistics.Add(stat);

            return await Task.FromResult(stat);
        }

        public Task DeleteStatisticAsync(Statistic statistic)
        {
            _statistics.Remove(statistic);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Statistic>> GetAllStatisticsAsync()
        {
            return await Task.FromResult(_statistics);
        }

        public async Task<Statistic> GetStatisticAsync(string name)
        {
            return await Task.FromResult(_statistics.FirstOrDefault(x => x.AliasesArray.Contains(name, StringComparer.OrdinalIgnoreCase)));
        }

        public async Task UpdateStatisticAsync(Statistic statistic)
        {
            await Task.Delay(0); // to get rid of warning
            _statistics[_statistics.FindIndex(x => x.Id.Equals(statistic.Id))] = statistic;
        }
    }
}
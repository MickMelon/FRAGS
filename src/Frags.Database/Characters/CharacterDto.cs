using Frags.Core.Common;
using Frags.Core.Statistics;
using Frags.Database.Statistics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Frags.Database.Characters
{
    /// <summary>
    /// The Character model.
    /// </summary>
    public class CharacterDto : BaseModel
    {
        //[Key]
        public string Id { get; set; }

        public ulong UserIdentifier { get; set; }

        public bool Active { get; set; }
        
        /// <summary>
        /// The character's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The character's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The character's story.
        /// </summary>
        public string Story { get; set; }
        
        /// <summary>
        /// The character's current experience.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// The character's statistics.
        /// </summary>
        [JsonIgnore]
        public IDictionary<Statistic, StatisticValue> Statistics
        {
            get
            {
                var dict = new Dictionary<Statistic, StatisticValue>();

                foreach (var stat in StatisticMappings)
                    dict.Add(stat.Statistic, stat.StatisticValue);
                    
                return dict;
            }
        }

        public IList<StatisticMapping> StatisticMappings { get; set; }
    }
}
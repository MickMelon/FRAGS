using Frags.Core.Common;
using Frags.Core.Effects;
using Frags.Core.Statistics;
using Frags.Database.Campaigns;
using Frags.Database.Effects;
using Frags.Database.Statistics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Frags.Database.Characters
{
    /// <summary>
    /// The Character model.
    /// </summary>
    public class CharacterDto : BaseModel
    {
        public int Id { get; set; }

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

        public CampaignDto Campaign { get; set; }
        public int? CampaignId { get; set; }

        
        [InverseProperty("Characters")]
        public UserDto User { get; private set; }

        /// <summary>
        /// The character's current amount of money.
        /// </summary>
        public int Money { get; set; }

        public int AttributePoints { get; set; }
        
        public int SkillPoints { get; set; }

        public virtual IList<StatisticMapping> Statistics { get; set; }

        public virtual IList<EffectMapping> EffectMappings { get; set; }
    }
}
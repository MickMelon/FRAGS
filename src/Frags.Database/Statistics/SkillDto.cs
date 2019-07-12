using System;
using Frags.Core.Common;

namespace Frags.Database.Statistics
{
    /// <summary>
    /// The skill model.
    /// </summary>
    public class SkillDto : StatisticDto
    {
        protected SkillDto() : base() { }

        public SkillDto(AttributeDto attribute, string name, string description = "") : base(name, description)
        {
            Attribute = attribute;
        }

        /// <summary>
        /// The skill's associated attribute.
        /// 
        /// Example: A Persuasion skill might have Charisma as the attibute.
        /// </summary>
        public AttributeDto Attribute { get; set; }

        /// <summary>
        /// An optional minimum value to make use of this skill.
        /// </summary>
        public int MinimumValue { get; set; }
    }
}
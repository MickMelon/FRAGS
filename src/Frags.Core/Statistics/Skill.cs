using System;
using Frags.Core.Common;

namespace Frags.Core.Statistics
{
    /// <summary>
    /// The skill model.
    /// </summary>
    public class Skill : Statistic
    {
        public Skill(Attribute attribute, string id, string name, string description = "") : base(id, name, description)
        {
            Attribute = attribute;
        }

        /// <summary>
        /// The skill's associated attribute.
        /// 
        /// Example: A Persuasion skill might have Charisma as the attibute.
        /// </summary>
        public Attribute Attribute { get; set; }
    }
}
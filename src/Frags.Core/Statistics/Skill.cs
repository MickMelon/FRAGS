using System;
using Frags.Core.Common;

namespace Frags.Core.Statistics
{
    /// <summary>
    /// The skill model.
    /// </summary>
    public class Skill : Statistic
    {
        private Skill(string id, string name) : base(id, name) { }

        public Skill(Attribute attribute, string name, string description = "") : base(name, description)
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
using System;
using Frags.Core.Common;

namespace Frags.Core.Statistics
{
    /// <summary>
    /// The skill model.
    /// </summary>
    public class Skill : Statistic
    {
        /// <summary>
        /// The skill's associated attribute.
        /// 
        /// Example: A Persuasion skill might have Charisma as the attibute.
        /// </summary>
        public Attribute Attribute { get; set; }
    }
}
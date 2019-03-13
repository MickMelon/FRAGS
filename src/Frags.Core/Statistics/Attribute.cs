using System;
using Frags.Core.Common;

namespace Frags.Core.Statistics
{
    /// <summary>
    /// The attribute model.
    /// </summary>
    public class Attribute : Statistic
    {
        public Attribute(string name, string description = "") : base(name, description)
        {
        }
    }
}
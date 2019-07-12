using System;
using Frags.Core.Common;

namespace Frags.Database.Statistics
{
    /// <summary>
    /// The attribute model.
    /// </summary>
    public class AttributeDto : StatisticDto
    {
        protected AttributeDto() : base() { }

        public AttributeDto(string name, string description = "") : base(name, description)
        {
        }
    }
}
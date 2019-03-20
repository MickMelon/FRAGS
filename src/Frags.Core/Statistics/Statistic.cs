using System;
using Frags.Core.Common;

namespace Frags.Core.Statistics
{
    /// <summary>
    /// The statistic model.
    /// </summary>
    public abstract class Statistic : BaseModel
    {
        /// <summary>
        /// The statistics's unique identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The statistics's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The statistics's description.
        /// </summary>
        public string Description { get; set; }

        protected Statistic(string name, string description = "")
        {
            Name = name;
            Description = description;
        }

        protected Statistic() { }
    }
}
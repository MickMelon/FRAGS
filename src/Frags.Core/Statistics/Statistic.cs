using System;
using Frags.Core.Common;

namespace Frags.Core.Statistics
{
    /// <summary>
    /// The statistic model.
    /// </summary>
    public abstract class Statistic
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

        protected Statistic(string id, string name, string description = "")
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
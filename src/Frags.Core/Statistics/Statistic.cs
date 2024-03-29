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
        public int Id { get; set; }

        /// <summary>
        /// The statistics's name.
        /// </summary>
        public string Name { get; set; }

        public string Aliases { get; set; }

        public string[] AliasesArray
        {
            get
            {
                if (!String.IsNullOrEmpty(Aliases))
                    return Aliases.Split('/');
                else
                    return new string[] { Name };
            }
        }

        /// <summary>
        /// The statistics's description.
        /// </summary>
        public string Description { get; set; }

        public int Order { get; set; }

        protected Statistic(string name, string description = "")
        {
            Name = name;
            Aliases = name + "/";
            Description = description;
        }

        protected Statistic() { }
    }
}
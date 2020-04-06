using System;
using Frags.Core.Campaigns;
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

        public Campaign Campaign { get; set; }

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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Id == ((Statistic)obj).Id;
        }
        
        public override int GetHashCode()
        {
            return Id;
        }

        protected Statistic(string name, string description = "")
        {
            Name = name;
            Aliases = name + "/";
            Description = description;
        }

        protected Statistic() { }
    }
}
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
        /// It is always unique, even between Campaigns.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The statistics's name.
        /// </summary>
        public string Name { get; set; }

        public string Aliases { get; set; }

        public Campaign Campaign { get; set; }

        /// <summary>
        /// An integer representing where the statistic should be placed in a sorted list.
        /// </summary>
        public int Order { get; set; }

        public static readonly char ALIAS_SEPARATOR = '/';

        public string[] AliasesArray
        {
            get
            {
                if (!String.IsNullOrEmpty(Aliases))
                    return Aliases.Split(ALIAS_SEPARATOR);
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

            Statistic stat = (Statistic)obj;

            return Name.Equals(stat.Name) && 
            Description.Equals(stat.Description) && 
            Aliases.Equals(stat.Aliases) &&
            Id.Equals(stat.Id);
        }
        
        public override int GetHashCode()
        {
            return ("" + Name + Description + Aliases + Id).GetHashCode();
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
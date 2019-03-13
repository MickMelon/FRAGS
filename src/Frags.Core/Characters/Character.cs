using System;
using System.Collections.Generic;
using Frags.Core.Common;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;

namespace Frags.Core.Characters
{
    /// <summary>
    /// The Character model.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// The character's unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The character's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The character's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The character's story.
        /// </summary>
        public string Story { get; set; }
        
        /// <summary>
        /// The character's current experience.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// The character's level calculated from the experience.
        /// </summary>
        public int Level { get => Character.GetLevelFromExperience(Experience); }

        /// <summary>
        /// The character's statistics.
        /// </summary>
        public IDictionary<Statistic, StatisticValue> Statistics { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Character" /> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="story"></param>
        public Character(int id, string name, string description = "", string story = "")
        {
            Id = id;
            Name = name;
            Description = description;
            Story = story;
        }

        /// <summary>
        /// Rolls the specified skill for the character.
        /// </summary>
        /// <param name="skill">The skill name.</param>
        /// <returns>What the character rolled.</returns>
        public int Roll(string skill)
        {
            return GameRandom.D20();
        }

        /// <summary>
        /// Rolls the specified statistic for the character.
        /// </summary>
        /// <param name="stat">The statistic name.</param>
        /// <returns>What the character rolled.</returns>
        public double? RollStatistic(Statistic stat, IRollStrategy strategy)
        {
            if (stat == null || strategy == null || Statistics == null) return null;

            if (Statistics.TryGetValue(stat, out StatisticValue value))
            {
                return strategy.RollStatistic(stat, this);
            }
            
            return null;
        }

        public static int GetLevelFromExperience(int experience) =>
            Convert.ToInt32((Math.Sqrt(experience + 125) / (10 * Math.Sqrt(5))));
    }
}
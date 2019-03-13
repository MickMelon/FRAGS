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
        public string Id { get; private set; }
        
        /// <summary>
        /// The unique identifier of the user that owns the character.
        /// </summary>
        /// <remarks>
        /// Currently it's the DiscordID, but has been named UserIdentifier just
        /// in case Discord is changed for something else one day.
        /// </remarks>
        public ulong UserIdentifier { get; private set; }

        /// <summary>
        /// Whether this character is the user's active character.
        /// </summary>
        public bool Active { get; set; }

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
        /// <param name="userIdentifier">The unique identifier of user that owns character.</param>
        /// <param name="name">The character's name.</param>
        public Character(ulong userIdentifier, string name)
        {
            UserIdentifier = userIdentifier;
            Name = name;

            Statistics = new Dictionary<Statistic, StatisticValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Character" /> class.
        /// </summary>
        /// <param name="id">The character's unique identifier.</param>
        /// <param name="userIdentifier">The unique identifier of user that owns character.</param>
        /// <param name="active">Whether this is the active character.</param>
        /// <param name="name">The character's name.</param>
        /// <param name="description">The character's description.</param>
        /// <param name="story">The character's story.</param>
        public Character(string id, ulong userIdentifier, bool active, string name,
            string description = "", string story = "")
        {
            Id = id;
            UserIdentifier = userIdentifier;
            Active = active;
            Name = name;
            Description = description;
            Story = story;

            Statistics = new Dictionary<Statistic, StatisticValue>();
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
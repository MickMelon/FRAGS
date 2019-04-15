using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Common;
using Frags.Core.Effects;
using Frags.Core.Game.Progression;
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

        public int AttributePoints { get; set; }

        public int SkillPoints { get; set; }

        /// <summary>
        /// The character's current amount of money.
        /// </summary>
        public int Money { get; set; }

        /// <summary>
        /// Where the character's statistics are actually stored.
        /// </summary>
        public IList<StatisticMapping> Statistics { get; set; }

        /// <summary>
        /// Clones the character's statistics and applies their current effects to it.
        /// </summary>
        /// <returns>A new statistic list with values reflecting their applied effects.</returns>
        public IList<StatisticMapping> GetEffectiveStatistics()
        {
            var effectiveStats = CloneStatistics();

            foreach (var effect in Effects)
            {
                foreach (var statEffect in effect.StatisticEffects)
                {
                    var match = effectiveStats.FirstOrDefault(x => x.Statistic.Equals(statEffect.Statistic));

                    if (match != null)
                        match.StatisticValue.Value += statEffect.StatisticValue.Value;
                }
            }

            return effectiveStats;
        }

        /// <summary>
        /// A list of which Effects are currently applied to this character.
        /// </summary>
        public virtual IList<Effect> Effects { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Character" /> class.
        /// </summary>
        /// <param name="userIdentifier">The unique identifier of user that owns character.</param>
        /// <param name="name">The character's name.</param>
        public Character(ulong userIdentifier, string name)
        {
            UserIdentifier = userIdentifier;
            Name = name;

            Statistics = new List<StatisticMapping>();
            Effects = new List<Effect>();
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

            Statistics = new List<StatisticMapping>();
            Effects = new List<Effect>();
        }

        /// <summary>
        /// Retrives the specified StatisticValue from the character's Statistics if it exists.
        /// </summary>
        /// <param name="stat">The statistic to retrieve.</param>
        /// <returns>A StatisticValue associated with the specified Statistic.</returns>
        public StatisticValue GetStatistic(Statistic stat, bool useEffects = false) =>
            useEffects ? GetEffectiveStatistics()?.FirstOrDefault(x => x.Statistic.Equals(stat))?.StatisticValue :
            Statistics?.FirstOrDefault(x => x.Statistic.Equals(stat))?.StatisticValue;

        /// <summary>
        /// Sets the specified Statistic to the given StatisticValue if it exists.
        /// Otherwise, add a new StatisticMapping entry to the list.
        /// </summary>
        /// <param name="stat">The statistic to set.</param>
        /// <param name="newValue">The StatisticValue to associate with the Statistic.</param>
        public void SetStatistic(Statistic stat, StatisticValue newValue)
        {
            var statMap = Statistics.FirstOrDefault(x => x.Statistic.Equals(stat));
            if (statMap == null) 
            {
                Statistics.Add(new StatisticMapping(stat, newValue));
                return;
            }

            statMap.StatisticValue = newValue;
        }

        /// <summary>
        /// Deep clones the character's current statistics.
        /// </summary>
        /// <returns>A new list containing clones of the character's statistics.</returns>
        private List<StatisticMapping> CloneStatistics()
        {
            var newStats = new List<StatisticMapping>();

            foreach (var stat in Statistics)
                newStats.Add(new StatisticMapping(stat.Statistic, stat.StatisticValue));

            return newStats;
        }
    }
}
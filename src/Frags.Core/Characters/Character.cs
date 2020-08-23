using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Common;
using Frags.Core.Effects;
using Frags.Core.Game.Progression;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Attribute = Frags.Core.Statistics.Attribute;

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
        public int Id { get; private set; }

        /// <summary>
        /// The unique identifier of the user that owns the character.
        /// </summary>
        /// <remarks>
        /// Currently it's the DiscordID, but has been named UserIdentifier just
        /// in case Discord is changed for something else one day.
        /// </remarks>
        public User User { get; private set; }

        /// <summary>
        /// Whether this character is the user's active character.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The optional campaign this character is a part of.
        /// </summary>
        public Campaign Campaign { get; set; }
        public int? CampaignId { get; set; }

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
        public long Money { get; set; } = 1000;

        /// <summary>
        /// Where the character's statistics are actually stored.
        /// </summary>
        public Dictionary<Statistic, StatisticValue> Statistics { get; set; }

        public Dictionary<Attribute, StatisticValue> Attributes => 
            Statistics.Where(x => x.Key is Attribute).ToDictionary(x => (Attribute)x.Key, x => x.Value);

        public Dictionary<Skill, StatisticValue> Skills =>
            Statistics.Where(x => x.Key is Skill).ToDictionary(x => (Skill)x.Key, x => x.Value);

        /// <summary>
        /// Clones the character's statistics and applies their current effects to it.
        /// </summary>
        /// <returns>A new statistic list with values reflecting their applied effects.</returns>
        public Dictionary<Statistic, StatisticValue> GetEffectiveStatistics()
        {
            var clonedStats = CloneStatistics();

            foreach (var effect in Effects)
                foreach (var statEffect in effect.StatisticEffects)
                    if (clonedStats.ContainsKey(statEffect.Key))
                        clonedStats[statEffect.Key].Value += statEffect.Value.Value;

            return clonedStats;
        }

        /// <summary>
        /// A list of which Effects are currently applied to this character.
        /// </summary>
        public virtual IList<Effect> Effects { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Character" /> class.
        /// </summary>
        /// <param name="user">The unique identifier of user that owns character.</param>
        /// <param name="name">The character's name.</param>
        public Character(User user, string name)
        {
            User = user;
            Name = name;

            Statistics = new Dictionary<Statistic, StatisticValue>();
            Effects = new List<Effect>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Character" /> class.
        /// </summary>
        /// <param name="id">The character's unique identifier.</param>
        /// <param name="user">The unique identifier of user that owns character.</param>
        /// <param name="active">Whether this is the active character.</param>
        /// <param name="name">The character's name.</param>
        /// <param name="description">The character's description.</param>
        /// <param name="story">The character's story.</param>
        public Character(int id, User user, bool active, string name,
            string description = "", string story = "")
        {
            Id = id;
            User = user;
            Active = active;
            Name = name;
            Description = description;
            Story = story;

            Statistics = new Dictionary<Statistic, StatisticValue>();
            Effects = new List<Effect>();
        }

        private Character() 
        {
            Statistics = new Dictionary<Statistic, StatisticValue>();
            Effects = new List<Effect>();
        }

        /// <summary>
        /// Retrives the specified StatisticValue from the character's Statistics if it exists.
        /// </summary>
        /// <param name="stat">The statistic to retrieve.</param>
        /// <returns>A StatisticValue associated with the specified Statistic, or null if not found.</returns>
        public StatisticValue GetStatistic(Statistic stat, bool useEffects = false)
        {
            if (stat == null) return null;

            StatisticValue result = null;

            if (useEffects)
            {
                GetEffectiveStatistics().TryGetValue(stat, out result);
            }
            else
            {
                Statistics.TryGetValue(stat, out result);
            }

            return result;
        }

        /// <summary>
        /// Sets the specified Statistic to the given StatisticValue if it exists.
        /// Otherwise, add a new StatisticMapping entry to the list.
        /// </summary>
        /// <param name="stat">The statistic to set.</param>
        /// <param name="newValue">The StatisticValue to associate with the Statistic.</param>
        public void SetStatistic(Statistic stat, StatisticValue newValue)
        {
            if (Statistics.ContainsKey(stat))
            {
                Statistics[stat] = newValue;
            }
            else
            {
                Statistics.Add(stat, newValue);
            }
        }

        /// <summary>
        /// Deep clones the character's current statistics.
        /// </summary>
        /// <returns>A new list containing clones of the character's statistics.</returns>
        private Dictionary<Statistic, StatisticValue> CloneStatistics()
        {
            var newStats = new Dictionary<Statistic, StatisticValue>();

            foreach (var stat in Statistics)
            {
                var statVal = stat.Value;
                newStats.Add(stat.Key, new StatisticValue(statVal.Value, statVal.IsProficient, statVal.Proficiency));
            }

            return newStats;
        }
    }
}
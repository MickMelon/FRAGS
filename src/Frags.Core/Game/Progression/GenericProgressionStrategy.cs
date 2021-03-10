using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Common.Exceptions;
using Frags.Core.DataAccess;
using Frags.Core.Statistics;
using Attribute = Frags.Core.Statistics.Attribute;

namespace Frags.Core.Game.Progression
{
    public class GenericProgressionStrategy : IProgressionStrategy
    {
        private readonly ICampaignProvider _campProvider;
        private readonly IStatisticProvider _statProvider;
        private readonly StatisticOptions _statOptions;

        public GenericProgressionStrategy(IStatisticProvider statProvider, StatisticOptions statOptions, ICampaignProvider campProvider)
        {
            _statProvider = statProvider;
            _statOptions = statOptions;
            _campProvider = campProvider;
        }

        protected Character currentChar = null;
        protected StatisticOptions statOptCache = null;
        protected IEnumerable<Statistic> statisticCache = null;

        protected async Task<StatisticOptions> GetStatisticOptions(Character character)
        {
            if (character == currentChar && statOptCache != null)
                return statOptCache;

            currentChar = character;

            if (character.Campaign != null)
                statOptCache = await _campProvider.GetStatisticOptionsAsync(character.Campaign);
            else
                statOptCache = _statOptions;

            return statOptCache;
        }

        protected async Task<IEnumerable<Statistic>> GetStatistics(Character character)
        {
            if (character == currentChar && statisticCache != null)
                return statisticCache;

            currentChar = character;

            if (character.Campaign != null)
                statisticCache = await _statProvider.GetAllStatisticsFromCampaignAsync(character.Campaign);
            else
                statisticCache = await _statProvider.GetAllStatisticsAsync();

            return statisticCache;
        }

        /// <summary>
        /// Calculate the experience required for a level.
        /// </summary>
        private int CalculateExperienceForLevel(int level)
        {
            if (level < 1 || level > 1000) return -1;
            return (level * (level - 1) / 2) * 1000;
        }

        public int GetCharacterLevel(Character character)
        {
            if (character.Experience == 0) return 1;
            int level = Convert.ToInt32(Math.Sqrt(character.Experience + 125) / (10 * Math.Sqrt(5)));
            return level;
        }

        protected async Task InitializeStatistics(Character character)
        {
            IEnumerable<Statistic> stats = await GetStatistics(character);
            StatisticOptions statOpts = await GetStatisticOptions(character);

            foreach (var stat in stats)
            {
                if (character.GetStatistic(stat) == null)
                {
                    if (stat is Attribute)
                        character.SetStatistic(stat, new StatisticValue(statOpts.InitialAttributeMin));
                    if (stat is Skill)
                        character.SetStatistic(stat, new StatisticValue(statOpts.InitialSkillMin));
                }
            }
        }

        virtual public async Task<bool> SetStatistic(Character character, Statistic statistic, int? newValue)
        {
            StatisticOptions statOpts = await GetStatisticOptions(character);

            await InitializeStatistics(character);

            var level = GetCharacterLevel(character);
            if (!newValue.HasValue) throw new ProgressionException(Messages.INVALID_INPUT);
            if (level <= (statOpts).InitialSetupMaxLevel || !await InitialAttributesSet(character) || !await InitialSkillsSet(character))
                return await SetInitialStatistic(character, statistic, newValue.Value);

            // Character is above setup level and has their attributes & skills set
            if (statistic is Attribute attribute)
            {
                var current = character.GetStatistic(attribute);

                if (newValue.Value < current.Value) throw new ProgressionException(Messages.INVALID_INPUT);
                if (newValue.Value > (statOpts).AttributeMax) throw new ProgressionException(Messages.TOO_HIGH);

                if (character.AttributePoints + current.Value - newValue.Value >= 0)
                {
                    int amt = newValue.Value - current.Value;

                    if (current.IsProficient)
                        newValue += (int)(amt * (statOpts).ProficientAttributeMultiplier);

                    current.Value = newValue.Value;

                    character.AttributePoints -= amt;
                }
                else
                {
                    throw new ProgressionException(Messages.NOT_ENOUGH_POINTS);
                }

                return true;
            }
            else if (statistic is Skill skill)
            {
                var current = character.GetStatistic(skill);

                if (newValue.Value < current.Value) throw new ProgressionException(Messages.INVALID_INPUT);
                if (newValue.Value > (statOpts).SkillMax) throw new ProgressionException(Messages.TOO_HIGH);

                if (character.SkillPoints + current.Value - newValue.Value >= 0)
                {
                    int amt = newValue.Value - current.Value;

                    if (current.IsProficient)
                        newValue += (int)(amt * (statOpts).ProficientAttributeMultiplier);

                    current.Value = newValue.Value;

                    character.SkillPoints -= amt;
                }
                else
                {
                    throw new ProgressionException(Messages.NOT_ENOUGH_POINTS);
                }

                return true;
            }

            return false;
        }

        protected async Task<bool> SetInitialStatistic(Character character, Statistic statistic, int newValue)
        {
            int statMin, statMax, statsAtMax, points, level = GetCharacterLevel(character);
            // This is either an enumerable of all the character's attributes or skills, never both.
            IEnumerable<int> stats;
            StatisticOptions statOpts = await GetStatisticOptions(character);

            // Set variables
            if (statistic is Attribute attrib)
            {
                statMin = statOpts.InitialAttributeMin; 
                statMax = statOpts.InitialAttributeMax; 
                points = statOpts.InitialAttributePoints;
                statsAtMax = statOpts.InitialAttributesAtMax;

                stats = character.Attributes.Select(x => x.Value.Value);

                if (level > statOpts.InitialSetupMaxLevel && await InitialAttributesSet(character))
                    throw new ProgressionException(Messages.CHAR_LEVEL_TOO_HIGH);
            }
            else
            {
                statMin = statOpts.InitialSkillMin;
                statMax = statOpts.InitialSkillMax;
                points = statOpts.InitialSkillPoints;
                statsAtMax = statOpts.InitialSkillsAtMax;

                stats = character.Skills.Select(x => x.Value.Value);

                if (level > statOpts.InitialSetupMaxLevel && await InitialSkillsSet(character)) 
                    throw new ProgressionException(Messages.CHAR_LEVEL_TOO_HIGH);
            }

            if (newValue < statMin) throw new ProgressionException(Messages.TOO_LOW);
            if (newValue > statMax) throw new ProgressionException(Messages.TOO_HIGH);
            
            int sum = stats.Sum(x => x);

            StatisticValue currentVal = character.GetStatistic(statistic);
            if (currentVal == null) currentVal = new StatisticValue(0);

            // Make sure the character has enough remaining points to do that 
            // (we refund the current stat value since we're overwriting it)
            var newSum = sum - currentVal.Value + newValue;
            if (points - newSum < 0) 
                throw new ProgressionException(string.Format(Messages.STAT_NOT_ENOUGH_POINTS, newSum, points));

            // Check if they go over the limit for attributes set to the max
            // Example: InitialAttributesAtMax is set to 2 and InitialAttributeMax is set to 10
            // If we already have 2 attributes with a value of 10 and we try to set a third, disallow it.
            if (statsAtMax > 0 &&
                newValue == statMax &&
                    stats.Count(x => x == statMax) + 1 > statsAtMax)
                        throw new ProgressionException(String.Format(Messages.STAT_TOO_MANY_AT_MAX, statsAtMax));

            currentVal.Value = newValue;
            
            return true;
        }

        virtual public async Task<bool> SetProficiency(Character character, Statistic statistic, bool proficient)
        {
            StatisticOptions statOpts = await GetStatisticOptions(character);

            int alreadySet, level = GetCharacterLevel(character), initialProficient;
            if (statistic is Attribute attrib)
            {
                alreadySet = character.Attributes.Count(x => x.Value.IsProficient);
                initialProficient = (statOpts).InitialAttributesProficient;
            }
            else
            {
                alreadySet = character.Skills.Count(x => x.Value.IsProficient);
                initialProficient = (statOpts).InitialSkillsProficient;
            }

            if (level > (statOpts).InitialSetupMaxLevel && alreadySet >= initialProficient)
                throw new ProgressionException(Messages.CHAR_LEVEL_TOO_HIGH);

            if (proficient && alreadySet + 1 > initialProficient)
                throw new ProgressionException(Messages.NOT_ENOUGH_POINTS);

            character.GetStatistic(statistic).IsProficient = proficient;
            return true;
        }

        public async Task<bool> ResetCharacter(Character character)
        {
            var level = GetCharacterLevel(character);
            //if (level <= 1) return false;

            character.Statistics.Clear();
            await InitializeStatistics(character);

            character.AttributePoints = 0;
            character.SkillPoints = 0;

            await OnLevelUp(character, level - 1);

            return true;
        }

        protected async Task<bool> InitialAttributesSet(Character character)
        {
            IEnumerable<Statistic> stats = await GetStatistics(character);
            StatisticOptions statOpts = await GetStatisticOptions(character);

            if (character == null || character.Statistics == null) return false;

            var attribs = character.Attributes;
            var sum = attribs.Sum(x => x.Value.Value);

            // Character attributes don't match up with database attributes
            if (attribs.Count != stats.OfType<Attribute>().Count()) return false;
            // Not all attributes are above or equal their minimum vaulue
            if (attribs.Any(x => x.Value.Value < (statOpts).InitialAttributeMin)) return false;
            // Character has not set their initial attribute values
            if (sum < (statOpts).InitialAttributePoints) return false;
            
            return true;
        }

        protected async Task<bool> InitialSkillsSet(Character character)
        {
            IEnumerable<Statistic> stats = await GetStatistics(character);
            StatisticOptions statOpts = await GetStatisticOptions(character);

            if (character == null || character.Statistics == null) return false;

            var skills = character.Skills;
            var sum = skills.Sum(x => x.Value.Value);

            // Character attributes don't match up with database attributes
            if (skills.Count != stats.OfType<Skill>().Count()) return false;
            // Not all attributes are above or equal their minimum vaulue
            if (skills.Any(x => x.Value.Value < (statOpts).InitialSkillMin)) return false;
            // Character has not set their initial attribute values
            if (sum < (statOpts).InitialSkillPoints) return false;
            
            return true;
        }

        public async Task<bool> AddExperience(Character character, int amount)
        {
            int origLevel = GetCharacterLevel(character);

            character.Experience += amount;

            int newLevel = GetCharacterLevel(character);
            int difference = newLevel - origLevel;
            if (difference >= 1)
            {
                await OnLevelUp(character, difference);
                return true;
            }

            return false;
        }

        public async Task<bool> AddExperienceFromMessage(Character character, ulong channelId, string message)
        {
            IEnumerable<Statistic> stats = await GetStatistics(character);
            StatisticOptions statOpts = await GetStatisticOptions(character);

            if (!(statOpts).ExpEnabledChannels.Select(x => x.Id).Contains(channelId)) return false;
            if (string.IsNullOrWhiteSpace(message)) return false;

            return await AddExperience(character, message.Length / (statOpts).ExpMessageLengthDivisor);
        }

        virtual async protected Task OnLevelUp(Character character, int timesLeveledUp)
        {
            StatisticOptions statOpts = await GetStatisticOptions(character);

            for (int levelUp = 1; levelUp <= timesLeveledUp; levelUp++)
            {
                character.SkillPoints += (statOpts).SkillPointsOnLevelUp;
                character.AttributePoints += (statOpts).AttributePointsOnLevelUp;
            }
        }

        public async Task<string> GetCharacterInfo(Character character)
        {
            var level = GetCharacterLevel(character);
            StringBuilder output = new StringBuilder($"Remaining exp to level: {CalculateExperienceForLevel(level + 1) - character.Experience}\n");

            if (!await InitialAttributesSet(character))
                output.Append("Attributes are not set.\n");

            if (!await InitialSkillsSet(character))
                output.Append("Skills are not set.\n");

            return output.ToString();
        }

        public async Task<string> GetCharacterStatisticsInfo(Character character)
        {
            StatisticOptions statOpts = await GetStatisticOptions(character);

            var level = GetCharacterLevel(character);
            StringBuilder output = new StringBuilder();

            if (!await InitialAttributesSet(character))
            {
                var sum = character.Attributes.Sum(x => x.Value.Value);
                output.Append($"Remaining initial attribute points: {(statOpts).InitialAttributePoints - sum}\n");
            }

            if (!await InitialSkillsSet(character))
            {
                var sum = character.Skills.Sum(x => x.Value.Value);
                output.Append($"Remaining initial skill points: {(statOpts).InitialSkillPoints - sum}\n");
            }

            return output.ToString();
        }
    }
}
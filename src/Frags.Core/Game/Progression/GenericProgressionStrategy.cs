using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private readonly IStatisticProvider _statProvider;
        private readonly StatisticOptions _statOptions;

        public GenericProgressionStrategy(IStatisticProvider statProvider, StatisticOptions statOptions)
        {
            _statProvider = statProvider;
            _statOptions = statOptions;
        }

        public int GetCharacterLevel(Character character)
        {
            if (character.Experience == 0) return 1;
            int level = Convert.ToInt32(Math.Sqrt(character.Experience + 125) / (10 * Math.Sqrt(5)));
            return level;
        }

        protected async Task InitializeStatistics(Character character)
        {
            foreach (var stat in await _statProvider.GetAllStatisticsAsync())
            {
                if (character.GetStatistic(stat) == null)
                {
                    if (stat is Attribute)
                        character.SetStatistic(stat, new StatisticValue(_statOptions.InitialAttributeMin));
                    if (stat is Skill)
                        character.SetStatistic(stat, new StatisticValue(_statOptions.InitialSkillMin));
                }
            }
        }

        public async Task<bool> SetStatistic(Character character, Statistic statistic, int? newValue)
        {
            await InitializeStatistics(character);

            var level = GetCharacterLevel(character);
            if (!newValue.HasValue) throw new ProgressionException(Messages.INVALID_INPUT);
            if (level <= _statOptions.InitialSetupMaxLevel || !await InitialAttributesSet(character) || !await InitialSkillsSet(character))
                return await SetInitialStatistic(character, statistic, newValue.Value);

            // Character is above setup level and has their attributes & skills set
            if (statistic is Attribute attribute)
            {
                var current = character.GetStatistic(attribute);
                if (newValue.Value < current.Value) throw new ProgressionException(Messages.INVALID_INPUT);
                if (character.AttributePoints + current.Value - newValue.Value >= 0)
                {
                    if (current.IsProficient)
                        current.Value += Convert.ToInt32(newValue.Value * _statOptions.ProficientAttributeMultiplier);
                    else
                        current.Value += newValue.Value;

                    character.AttributePoints -= newValue.Value;
                    character.SetStatistic(attribute, current);
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
                if (character.SkillPoints + current.Value - newValue.Value >= 0)
                {
                    if (current.IsProficient)
                        current.Value += Convert.ToInt32(newValue.Value * _statOptions.ProficientSkillMultiplier);
                    else
                        current.Value += newValue.Value;

                    character.SkillPoints -= newValue.Value;
                    character.SetStatistic(skill, current);
                }
                else
                {
                    throw new ProgressionException(Messages.NOT_ENOUGH_POINTS);
                }

                return true;
            }

            return false;
        }

        private async Task<bool> SetInitialStatistic(Character character, Statistic statistic, int newValue)
        {
            int statMin, statMax, statsAtMax, points, level = GetCharacterLevel(character);
            // This is either an array of all the character's attributes or skills, never both.
            int[] stats;

            // Set variables
            if (statistic is Attribute attrib)
            {
                statMin = _statOptions.InitialAttributeMin; 
                statMax = _statOptions.InitialAttributeMax; 
                points = _statOptions.InitialAttributePoints;
                statsAtMax = _statOptions.InitialAttributesAtMax; 
                
                stats = character.Statistics.Where(x => x.Statistic is Attribute).Select(x => x.StatisticValue.Value).ToArray();

                if (level > _statOptions.InitialSetupMaxLevel && await InitialAttributesSet(character)) 
                    throw new ProgressionException(Messages.CHAR_LEVEL_TOO_HIGH);
            }
            else
            {
                statMin = _statOptions.InitialSkillMin;
                statMax = _statOptions.InitialSkillMax;
                points = _statOptions.InitialSkillPoints;
                statsAtMax = _statOptions.InitialSkillsAtMax;
                
                stats = character.Statistics.Where(x => x.Statistic is Skill).Select(x => x.StatisticValue.Value).ToArray();

                if (level > _statOptions.InitialSetupMaxLevel && await InitialSkillsSet(character)) 
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
            character.SetStatistic(statistic, currentVal);
            
            return true;
        }

        public Task<bool> SetProficiency(Character character, Statistic statistic, bool proficient)
        {
            int alreadySet, level = GetCharacterLevel(character);
            if (statistic is Attribute attrib)
            {
                alreadySet = character.Statistics.Where(x => x.Statistic is Attribute).Count(x => x.StatisticValue.IsProficient);
                if (level > _statOptions.InitialSetupMaxLevel && alreadySet >= _statOptions.InitialAttributesProficient) 
                    throw new ProgressionException(Messages.CHAR_LEVEL_TOO_HIGH);

                if (proficient && alreadySet + 1 > _statOptions.InitialAttributesProficient)
                    throw new ProgressionException(Messages.NOT_ENOUGH_POINTS);
            }
            else
            {
                alreadySet = character.Statistics.Where(x => x.Statistic is Skill).Count(x => x.StatisticValue.IsProficient);
                if (level > _statOptions.InitialSetupMaxLevel && alreadySet >= _statOptions.InitialSkillsProficient) 
                    throw new ProgressionException(Messages.CHAR_LEVEL_TOO_HIGH);

                if (proficient && alreadySet + 1 > _statOptions.InitialSkillsProficient)
                    throw new ProgressionException(Messages.NOT_ENOUGH_POINTS);
            }

            character.GetStatistic(statistic).IsProficient = proficient;
            return Task.FromResult(true);
        }

        public Task<bool> ResetCharacter(Character character)
        {
            var level = GetCharacterLevel(character);
            if (level <= 1) return Task.FromResult(false);

            foreach (var stat in character.Statistics)
            {
                if (stat.Statistic is Attribute)
                    stat.StatisticValue.Value = _statOptions.InitialAttributeMin;
                if (stat.Statistic is Skill)
                    stat.StatisticValue.Value = _statOptions.InitialSkillMin;

                stat.StatisticValue.IsProficient = false;
                stat.StatisticValue.Proficiency = 0;
            }

            character.AttributePoints = 0;
            character.SkillPoints = 0;

            OnLevelUp(character, level - 1);

            return Task.FromResult(true);
        }

        private async Task<bool> InitialAttributesSet(Character character)
        {
            if (character == null || character.Statistics == null) return false;

            var attribs = character.Statistics.Where(x => x.Statistic is Attribute).ToDictionary(x => (Attribute)x.Statistic, x => x.StatisticValue);
            var sum = attribs.Sum(x => x.Value.Value);

            // Character attributes don't match up with database attributes
            if (attribs.Count != (await _statProvider.GetAllStatisticsAsync()).OfType<Attribute>().Count()) return false;
            // Not all attributes are above or equal their minimum vaulue
            if (attribs.Any(x => x.Value.Value < _statOptions.InitialAttributeMin)) return false;
            // Character has not set their initial attribute values
            if (sum < _statOptions.InitialAttributePoints) return false;
            
            return true;
        }

        private async Task<bool> InitialSkillsSet(Character character)
        {
            if (character == null || character.Statistics == null) return false;

            var skills = character.Statistics.Where(x => x.Statistic is Skill).ToDictionary(x => (Skill)x.Statistic, x => x.StatisticValue);
            var sum = skills.Sum(x => x.Value.Value);

            // Character attributes don't match up with database attributes
            if (skills.Count != (await _statProvider.GetAllStatisticsAsync()).OfType<Skill>().Count()) return false;
            // Not all attributes are above or equal their minimum vaulue
            if (skills.Any(x => x.Value.Value < _statOptions.InitialSkillMin)) return false;
            // Character has not set their initial attribute values
            if (sum < _statOptions.InitialSkillPoints) return false;
            
            return true;
        }

        public Task<bool> AddExperience(Character character, ulong channelId, string message)
        {
            int origLevel = GetCharacterLevel(character);

            if (!_statOptions.ExpEnabledChannels.Contains(channelId)) return Task.FromResult(false);
            if (string.IsNullOrWhiteSpace(message)) return Task.FromResult(false);

            character.Experience += message.Length;

            int newLevel = GetCharacterLevel(character);
            int difference = newLevel - origLevel;
            if (difference >= 1)
            {
                OnLevelUp(character, difference);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        private void OnLevelUp(Character character, int timesLeveledUp)
        {
            for (int levelUp = 1; levelUp <= timesLeveledUp; levelUp++)
            {
                character.SkillPoints += _statOptions.SkillPointsOnLevelUp;
                character.AttributePoints += _statOptions.AttributePointsOnLevelUp;
            }
        }
    }
}
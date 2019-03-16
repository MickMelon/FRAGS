using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Common.Exceptions;
using Frags.Core.DataAccess;
using Frags.Core.Statistics;
using Attribute = Frags.Core.Statistics.Attribute;

namespace Frags.Core.Game.Statistics
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

        public async Task<bool> SetStatistic(Character character, Statistic statistic, int? newValue)
        {
            if (!newValue.HasValue) throw new ProgressionException(Messages.INVALID_INPUT);

            int statMin, statMax, statsAtMax, points;
            // This is either a dictionary of all the character's attributes or skills, never both.
            Dictionary<Statistic, StatisticValue> stats;

            // Set variables
            if (statistic is Attribute attrib)
            {
                statMin = _statOptions.InitialAttributeMin; 
                statMax = _statOptions.InitialAttributeMax; 
                points = _statOptions.InitialAttributePoints;
                statsAtMax = _statOptions.InitialAttributesAtMax; 
                
                stats = character.Statistics.Where(x => x.Key is Attribute).ToDictionary(x => x.Key, x => x.Value);

                if (character.Level > _statOptions.InitialSetupMaxLevel && await InitialAttributesSet(character)) 
                    throw new ProgressionException(Messages.CHAR_LEVEL_TOO_HIGH);
            }
            else
            {
                statMin = _statOptions.InitialSkillMin;
                statMax = _statOptions.InitialSkillMax;
                points = _statOptions.InitialSkillPoints;
                statsAtMax = _statOptions.InitialSkillsAtMax;
                
                stats = character.Statistics.Where(x => x.Key is Skill).ToDictionary(x => x.Key, x => x.Value);

                if (character.Level > _statOptions.InitialSetupMaxLevel && await InitialSkillsSet(character)) 
                    throw new ProgressionException(Messages.CHAR_LEVEL_TOO_HIGH);
            }

            if (newValue < statMin) throw new ProgressionException(Messages.TOO_LOW);
            if (newValue > statMax) throw new ProgressionException(Messages.TOO_HIGH);
            
            int sum = stats.Sum(x => x.Value.Value);

            StatisticValue currentVal = new StatisticValue(0);
            // Save the old value for later
            bool containsStat = false;
            if (character.Statistics.ContainsKey(statistic))
            {
                currentVal = character.Statistics[statistic];
                containsStat = true;
            }

            // Make sure the character has enough remaining points to do that (we refund the current stat value since we're overwriting it)
            if (points - (sum - currentVal.Value + newValue) < 0) throw new ProgressionException(Messages.NOT_ENOUGH_POINTS);

            // Check if they go over the limit for attributes set to the max
            // Example: InitialAttributesAtMax is set to 2 and InitialAttributeMax is set to 10
            // If we already have 2 attributes with a value of 10 and we try to set a third, disallow it.
            if (statsAtMax > 0 &&
                newValue == statMax &&
                    stats.Count(x => x.Value.Value == statMax) + 1 > statsAtMax)
                        throw new ProgressionException(String.Format(Messages.STAT_TOO_MANY_AT_MAX, statsAtMax));

            if (containsStat)
                character.Statistics[statistic] = new StatisticValue(newValue.Value);
            else
                character.Statistics.Add(statistic, new StatisticValue(newValue.Value));
            
            return true;
        }

        public Task<bool> SetProficiency(Character character, Statistic statistic, bool proficient)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ResetCharacter(Character character)
        {
            throw new System.NotImplementedException();
        }

        private async Task<bool> InitialAttributesSet(Character character)
        {
            if (character == null || character.Statistics == null) return false;

            var attribs = character.Statistics.Where(x => x.Key is Attribute).ToDictionary(x => (Attribute)x.Key, x => x.Value);
            var sum = attribs.Sum(x => x.Value.Value);

            // Character attributes don't match up with database attributes
            if (attribs.Count != (await _statProvider.GetAllStatisticsAsync()).OfType<Attribute>().Count()) return false;
            // Character has not set their initial attribute values
            if (sum < _statOptions.InitialAttributePoints) return false;
            
            return true;
        }

        private async Task<bool> InitialSkillsSet(Character character)
        {
            if (character == null || character.Statistics == null) return false;

            var skills = character.Statistics.Where(x => x.Key is Skill).ToDictionary(x => (Skill)x.Key, x => x.Value);
            var sum = skills.Sum(x => x.Value.Value);

            // Character attributes don't match up with database attributes
            if (skills.Count != (await _statProvider.GetAllStatisticsAsync()).OfType<Skill>().Count()) return false;
            // Character has not set their initial attribute values
            if (sum < _statOptions.InitialSkillPoints) return false;
            
            return true;
        }
    }
}
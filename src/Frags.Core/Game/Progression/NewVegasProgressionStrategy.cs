using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Common.Exceptions;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Statistics;
using Attribute = Frags.Core.Statistics.Attribute;

namespace Frags.Core.Game.Progression
{
    public class NewVegasProgressionStrategy : GenericProgressionStrategy
    {
        private readonly StatisticOptions _statOptions;
        private readonly IStatisticProvider _statProvider;

        private static readonly int SKILL_BASE = 2;
        private static readonly int TAG_BONUS = 15;
        
        public NewVegasProgressionStrategy(IStatisticProvider statProvider, StatisticOptions statOptions) : base(statProvider, statOptions)
        {
            _statOptions = statOptions;
            _statProvider = statProvider;
        }

        override protected void OnLevelUp(Character character, int timesLeveledUp)
        {
            for (int levelUp = 1; levelUp <= timesLeveledUp; levelUp++)
            {
                int inte = character.Statistics.Where(x => x.Statistic.Name.EqualsIgnoreCase("intelligence")).FirstOrDefault()?.StatisticValue?.Value ?? 0;
                int origLevel = GetCharacterLevel(character) - timesLeveledUp;

                character.SkillPoints += _statOptions.SkillPointsOnLevelUp + inte / 2;

                if (inte % 2 != 0 && (origLevel + levelUp) % 2 == 0)
                    character.SkillPoints += 1;
            }
        }

        override public async Task<bool> SetStatistic(Character character, Statistic statistic, int? newValue)
        {
            await InitializeStatistics(character);

            var level = GetCharacterLevel(character);
            if (!newValue.HasValue) throw new ProgressionException(Messages.INVALID_INPUT);

            if (level <= _statOptions.InitialSetupMaxLevel || !await InitialAttributesSet(character))
                return await SetInitialStatistic(character, statistic, newValue.Value);

            // Character is above setup level and has their attributes & skills set
            if (statistic is Attribute attribute)
            {
                var current = character.GetStatistic(attribute);
                if (newValue.Value < current.Value) throw new ProgressionException(Messages.INVALID_INPUT);
                if (character.AttributePoints + current.Value - newValue.Value >= 0)
                {
                    int amt = newValue.Value - current.Value;

                    if (current.IsProficient)
                        newValue += (int)(amt * _statOptions.ProficientAttributeMultiplier);
                        
                    current.Value = newValue.Value;

                    character.AttributePoints -= amt;
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
                    int amt = newValue.Value - current.Value;

                    if (current.IsProficient)
                        newValue += (int)(amt * _statOptions.ProficientSkillMultiplier);
                    
                    current.Value = newValue.Value;

                    character.SkillPoints -= amt;
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

        new private async Task InitializeStatistics(Character character)
        {
            foreach (var stat in await _statProvider.GetAllStatisticsAsync())
            {
                if (character.GetStatistic(stat) == null)
                {
                    if (stat is Attribute)
                        character.SetStatistic(stat, new StatisticValue(1));
                    if (stat is Skill)
                        character.SetStatistic(stat, new StatisticValue(0));
                }
            }
        }

        new private async Task<bool> SetInitialStatistic(Character character, Statistic statistic, int value)
        {
            if (statistic is Skill) throw new ProgressionException("You cannot set initial skill values with this strategy.");

            bool success = await base.SetInitialStatistic(character, statistic, value);

            if (success && await InitialAttributesSet(character))
            {
                int luck = character.Statistics.Where(x => x.Statistic.Name.EqualsIgnoreCase("luck")).FirstOrDefault()?.StatisticValue?.Value ?? 0;

                foreach (var skill in character.Statistics.Select(x => x.Statistic).OfType<Skill>())
                {
                    var special = character.GetStatistic(skill.Attribute);
                    character.SetStatistic(skill, new StatisticValue(SKILL_BASE + (special.Value * 2) + (luck / 2)));
                }
            }

            return success;
        }

        override public Task<bool> SetProficiency(Character character, Statistic statistic, bool proficient)
        {
            int alreadySet, level = GetCharacterLevel(character);

            if (statistic is Attribute attrib)
            {
                throw new ProgressionException("Can't tag SPECIALs.");
            }
            else
            {
                alreadySet = character.Statistics.Where(x => x.Statistic is Skill).Count(x => x.StatisticValue.IsProficient);

                if (level > _statOptions.InitialSetupMaxLevel && alreadySet >= _statOptions.InitialSkillsProficient)
                    throw new ProgressionException(Messages.CHAR_LEVEL_TOO_HIGH);

                if (proficient && alreadySet + 1 > _statOptions.InitialSkillsProficient)
                    throw new ProgressionException(Messages.NOT_ENOUGH_POINTS);
            }

            var statVal = character.GetStatistic(statistic);
            statVal.IsProficient = proficient;
            statVal.Value += TAG_BONUS;
            
            return Task.FromResult(true);
        }
    }
}

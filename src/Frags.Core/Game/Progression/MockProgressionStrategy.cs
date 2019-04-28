using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Statistics;

namespace Frags.Core.Game.Progression
{
    public class MockProgressionStrategy : IProgressionStrategy
    {
        public Task<bool> AddExperience(Character character, int amount)
        {
            character.Experience += amount;
            return Task.FromResult(true);
        }

        public Task<bool> AddExperienceFromMessage(Character character, ulong channelId, string message)
        {
            character.Experience += message.Length;
            return Task.FromResult(true);
        }

        public Task<string> GetCharacterInfo(Character character)
        {
            return Task.FromResult("");
        }

        public int GetCharacterLevel(Character character)
        {
            return character.Experience;
        }

        public Task<string> GetCharacterStatisticsInfo(Character character)
        {
            return Task.FromResult("");
        }

        public Task<bool> ResetCharacter(Character character)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SetProficiency(Character character, Statistic statistic, bool proficient)
        {
            character.GetStatistic(statistic).IsProficient = true;
            return Task.FromResult(true);
        }

        public Task<bool> SetStatistic(Character character, Statistic statistic, int? newValue = null)
        {
            character.SetStatistic(statistic, new StatisticValue(newValue.Value));
            return Task.FromResult(true);
        }
    }
}
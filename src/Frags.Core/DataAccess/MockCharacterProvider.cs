using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Models.Characters;

namespace Frags.Core.DataAccess
{
    /**
     * The real implementation of this class would be
     * contained in the Frags.Database project.    
     */
    public class MockCharacterProvider : ICharacterProvider
    {
        public async Task<bool> CreateCharacterAsync(ulong discordId, string name)
        {
            await Task.Delay(0);
            return true;
        }

        public async Task<Character> GetActiveCharacterAsync(ulong discordId)
        {
            await Task.Delay(0);
            return new Character()
            {
                Id = 1,
                Name = "New Character"
            };
        }

        public async Task<List<Character>> GetAllCharactersAsync(ulong discordId)
        {
            await Task.Delay(0);
            return new List<Character>();
        }

        public async Task UpdateCharacterAsync(Character character)
        {
            await Task.Delay(0);            
        }
    }
}
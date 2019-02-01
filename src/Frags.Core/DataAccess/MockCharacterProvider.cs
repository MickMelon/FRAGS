using System.Collections.Generic;
using System.Linq;
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
        private List<Character> _characters;

        public MockCharacterProvider()
        {
            _characters = new List<Character>()
            {
                new Character() { Id = 1, Name = "c1", Story = "s1", Description = "d1" },
                new Character() { Id = 2, Name = "c2", Story = "s2", Description = "d2" },
                new Character() { Id = 3, Name = "c3", Story = "s3", Description = "d3" },
                new Character() { Id = 4, Name = "c4", Story = "s4", Description = "d4" },
                new Character() { Id = 5, Name = "c5", Story = "s5", Description = "d5" }
            };
        }

        public async Task<bool> CreateCharacterAsync(ulong discordId, string name)
        {
            var character = new Character() { Id = (int)discordId, Name = name };
            _characters.Add(character);
            await Task.Delay(0); // just to ignore warning
            return true;
        }

        public async Task<Character> GetActiveCharacterAsync(ulong discordId)
        {
            await Task.Delay(0); // just to ignore warning
            return _characters.Where(c => c.Id == (int)discordId).FirstOrDefault();            
        }

        public async Task<List<Character>> GetAllCharactersAsync(ulong discordId)
        {
            await Task.Delay(0); // just to ignore warning
            return _characters.Where(c => c.Id == (int)discordId).ToList();
        }

        public async Task UpdateCharacterAsync(Character character)
        {
            var dbChar = _characters.Where(c => c.Id == (int)character.Id).FirstOrDefault();
            if (dbChar == null) return;
            
            dbChar = character;
            await Task.Delay(0);            
        }
    }
}
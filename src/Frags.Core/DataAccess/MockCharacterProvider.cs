using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;

namespace Frags.Core.DataAccess
{
    /**
     * The real implementation of this class would be
     * contained in the Frags.Database project.    
     */
    /// <inheritdoc/>
    public class MockCharacterProvider : ICharacterProvider
    {
        private List<Character> _characters;

        public MockCharacterProvider()
        {
            _characters = new List<Character>()
            {
                new Character(1,"c1", "s1", "d1"),
                new Character(2,"c2", "s2", "d2"),
                new Character(3,"c3", "s3", "d3"),
                new Character(4,"c4", "s4", "d4"),
                new Character(5,"c5", "s5", "d5")
            };
        }

        /// <inheritdoc/>
        public async Task<Character> CreateCharacterAsync(ulong discordId, string name)
        {
            var character = new Character((int)discordId, name);
            _characters.Add(character);
            return await Task.FromResult(character);
        }

        /// <inheritdoc/>
        public async Task<Character> GetActiveCharacterAsync(ulong discordId)
        {
            await Task.Delay(0); // just to ignore warning
            return _characters.Where(c => c.Id == (int)discordId).FirstOrDefault();            
        }

        /// <inheritdoc/>
        public async Task<List<Character>> GetAllCharactersAsync(ulong discordId)
        {
            await Task.Delay(0); // just to ignore warning
            return _characters.Where(c => c.Id == (int)discordId).ToList();
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            var dbChar = _characters.Where(c => c.Id == (int)character.Id).FirstOrDefault();
            if (dbChar == null) return;
            
            dbChar = character;
            await Task.Delay(0);            
        }
    }
}
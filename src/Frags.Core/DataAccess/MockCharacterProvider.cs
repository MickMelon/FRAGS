using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frags.Core.Characters;
using Frags.Core.Common;

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
                new Character(1, new User(1), true, "c1", "s1", "d1"),
                new Character(2, new User(2), true, "c2", "s2", "d2"),
                new Character(3, new User(3), true, "c3", "s3", "d3"),
                new Character(4, new User(4), true, "c4", "s4", "d4"),
                new Character(5, new User(5), true, "c5", "s5", "d5")
            };
        }

        /// <inheritdoc/>
        public async Task<bool> CreateCharacterAsync(ulong discordId, string name)
        {
            var character = new Character(new User(discordId), name);
            _characters.Add(character);
            return await Task.FromResult(true);
        }

        /// <inheritdoc/>
        public async Task<Character> GetActiveCharacterAsync(ulong discordId)
        {
            await Task.Delay(0); // just to ignore warning
            return _characters.Where(c => c.User.UserIdentifier == discordId).FirstOrDefault();            
        }

        /// <inheritdoc/>
        public async Task<List<Character>> GetAllCharactersAsync(ulong discordId)
        {
            await Task.Delay(0); // just to ignore warning
            return _characters.Where(c => c.User.UserIdentifier == discordId).ToList();
        }

        /// <inheritdoc/>
        public async Task UpdateCharacterAsync(Character character)
        {
            var dbChar = _characters.Where(c => c.User.UserIdentifier.Equals(character.User.UserIdentifier)).FirstOrDefault();
            if (dbChar == null) return;
            
            dbChar = character;
            await Task.Delay(0);            
        }
    }
}
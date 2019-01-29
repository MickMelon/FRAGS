using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Models.Characters;

namespace Frags.Core.DataAccess
{
    public interface ICharacterProvider
    {
        Task<bool> CreateCharacterAsync(ulong discordId, string name);
        Task<Character> GetActiveCharacterAsync(ulong discordId);
        Task<List<Character>> GetAllCharactersAsync(ulong discordId);
        Task UpdateCharacterAsync(Character character);
    }
}
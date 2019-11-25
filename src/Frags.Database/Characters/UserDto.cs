using Frags.Core.Common;
using Frags.Database.Campaigns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Frags.Database.Characters
{
    /// <summary>
    /// Keeps track of active characters in the database.
    /// </summary>
    public class UserDto : BaseModel
    {
        private UserDto()
        {
            Characters = new List<CharacterDto>();
        }

        public UserDto(ulong userIdentifier, CharacterDto activeCharacter = null) : this()
        {
            UserIdentifier = userIdentifier;
            ActiveCharacter = activeCharacter;

            if (activeCharacter != null)
                Characters.Add(activeCharacter);
        }

        public int Id { get; set; }

        public ulong UserIdentifier { get; set; }

        public ICollection<CharacterDto> Characters { get; set; }
        public CharacterDto ActiveCharacter { get; set; }
    }
}
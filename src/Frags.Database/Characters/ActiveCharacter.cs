using Frags.Database;
using System;
using System.ComponentModel.DataAnnotations;

namespace Frags.Database.Characters
{
    /// <summary>
    /// Keeps track of active characters in the database.
    /// </summary>
    public class ActiveCharacter : BaseModel
    {
        [Key]
        public ulong UserIdentifier { get; set; }

        public CharacterDto Character { get; set; }
    }
}
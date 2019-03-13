using Frags.Core.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Frags.Database.Characters
{
    /// <summary>
    /// Keeps track of active characters in the database.
    /// </summary>
    public class User : BaseModel
    {
        public string Id { get; set; }

        public ulong UserIdentifier { get; set; }

        public CharacterDto ActiveCharacter { get; set; }
    }
}
using Frags.Core.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Frags.Core.Characters
{
    /// <summary>
    /// Keeps track of active characters in the database.
    /// </summary>
    public class User : BaseModel
    {
        public int Id { get; set; }

        public ulong UserIdentifier { get; set; }

        public Character ActiveCharacter { get; set; }
        public int ActiveCharacterId { get; set; }
    }
}
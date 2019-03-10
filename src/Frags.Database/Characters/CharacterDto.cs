using Frags.Database;
using System;
using System.ComponentModel.DataAnnotations;

namespace Frags.Database.Characters
{
    /// <summary>
    /// The Character model.
    /// </summary>
    public class CharacterDto : BaseModel
    {
        //[Key]
        public string Id { get; set; }

        public ulong UserIdentifier { get; set; }

        public bool Active { get; set; }
        
        /// <summary>
        /// The character's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The character's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The character's story.
        /// </summary>
        public string Story { get; set; }
        
        /// <summary>
        /// The character's current experience.
        /// </summary>
        public int Experience { get; set; }
    }
}
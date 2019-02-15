using Frags.Database;
using System;

namespace Frags.Database.Characters
{
    /// <summary>
    /// The Character model.
    /// </summary>
    public class CharacterDto : BaseModel
    {
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
using System;
using Frags.Core.Common;

namespace Frags.Core.Characters
{
    /// <summary>
    /// The Character model.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// The character's unique identifier.
        /// </summary>
        public int Id { get; set; }

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

        /// <summary>
        /// The character's level calculated from the experience.
        /// </summary>
        public int Level { get => Character.GetLevelFromExperience(Experience); }

        public Character(int id, string name, string description = "", string story = "")
        {
            Id = id;
            Name = name;
            Description = description;
            Story = story;
        }

        /// <summary>
        /// Rolls the specified skill for the character.
        /// </summary>
        /// <param name="skill">The skill name.</param>
        /// <returns>What the character rolled.</returns>
        public int Roll(string skill)
        {
            return 46;
        }

        public static int GetLevelFromExperience(int experience) =>
            Convert.ToInt32((Math.Sqrt(experience + 125) / (10 * Math.Sqrt(5))));
    }
}
namespace Frags.Core.Models.Characters
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
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// The character's description.
        /// </summary>
        /// <value></value>
        public string Description { get; set; }

        /// <summary>
        /// The character's story.
        /// </summary>
        /// <value></value>
        public string Story { get; set; }
        
        /// <summary>
        /// The character's current experience.
        /// </summary>
        /// <value></value>
        public int Experience { get; set; }

        /// <summary>
        /// Rolls the specified skill for the character.
        /// </summary>
        /// <param name="skill">The skill name.</param>
        /// <returns>What the character rolled.</returns>
        public int Roll(string skill)
        {
            return 46;
        }
    }
}
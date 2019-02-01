using Frags.Core.Common.Attributes;

namespace Frags.Core.Controllers.ViewModels
{
    /// <summary>
    /// The ViewModel used for showing a character's details.
    /// </summary>
    [ViewModel]
    public class ShowCharacterViewModel
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
    }
}
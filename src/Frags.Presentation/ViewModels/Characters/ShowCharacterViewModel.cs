using Frags.Presentation.Attributes;

namespace Frags.Presentation.ViewModels.Characters
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
        /// The character's optional campaign.
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// The character's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The character's story.
        /// </summary>
        public string Story { get; set; }

        /// <summary>
        /// The character's level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The character's experience points.
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// The character's money.
        /// </summary>
        public long Money { get; set; }

        /// <summary>
        /// Important info related to the character provided by the ProgressionStrategy.
        /// </summary>
        public string ProgressionInformation { get; set; }
    }
}
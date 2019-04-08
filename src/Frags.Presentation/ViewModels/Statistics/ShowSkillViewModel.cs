using Frags.Presentation.Attributes;

namespace Frags.Presentation.ViewModels.Statistics
{
    /// <summary>
    /// The ViewModel used for showing a skill.
    /// </summary>
    [ViewModel]
    public class ShowSkillViewModel : ShowStatisticViewModel
    {
        public ShowSkillViewModel(string name, string desc, string[] aliases, int? value, bool? isProf, double? prof, int minimumValue, ShowAttributeViewModel attribute) : base(name, desc, aliases, value, isProf, prof)
        {
            MinimumValue = minimumValue;
            Attribute = attribute;
        }

        /// <summary>
        /// The skill's minimum value to use it.
        /// </summary>
        public int MinimumValue { get; set; }

        /// <summary>
        /// The skill's associated attribute.
        /// </summary>
        public ShowAttributeViewModel Attribute { get; set; }
    }
}
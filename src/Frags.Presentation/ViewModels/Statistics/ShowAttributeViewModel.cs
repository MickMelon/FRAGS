using Frags.Presentation.Attributes;

namespace Frags.Presentation.ViewModels.Statistics
{
    /// <summary>
    /// The ViewModel used for showing a attribute.
    /// </summary>
    [ViewModel]
    public class ShowAttributeViewModel : ShowStatisticViewModel
    {
        public ShowAttributeViewModel(string name, string desc, string[] aliases, int order, int? value, bool? isProf, double? prof) 
            : base(name, desc, aliases, order, value, isProf, prof)
        {
        }

        public ShowAttributeViewModel() { }
    }
}
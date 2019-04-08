using Frags.Presentation.Attributes;

namespace Frags.Presentation.ViewModels
{
    /// <summary>
    /// The ViewModel used for showing a attribute.
    /// </summary>
    [ViewModel]
    public class ShowAttributeViewModel : ShowStatisticViewModel
    {
        public ShowAttributeViewModel(string name, string desc, string[] aliases, int? value, bool? isProf, double? prof) 
            : base(name, desc, aliases, value, isProf, prof)
        {
        }

        public ShowAttributeViewModel() { }
    }
}
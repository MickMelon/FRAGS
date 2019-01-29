using Frags.Core.Common.Attributes;

namespace Frags.Core.Controllers.ViewModels
{
    [ViewModel]
    public class ShowCharacterViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Story { get; set; }
    }
}
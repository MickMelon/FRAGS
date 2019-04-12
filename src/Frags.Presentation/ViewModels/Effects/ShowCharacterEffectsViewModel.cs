using Frags.Presentation.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frags.Presentation.ViewModels.Effects
{
    [ViewModel]
    public class ShowCharacterEffectsViewModel
    {
        public ICollection<ShowEffectViewModel> Effects { get; set; }
    }
}

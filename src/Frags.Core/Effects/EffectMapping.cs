using Frags.Core.Characters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frags.Core.Effects
{
    public class EffectMapping
    {
        public string EffectId { get; set; }
        public Effect Effect { get; set; }

        public string CharacterId { get; set; }
        public Character Character { get; set; }
    }
}

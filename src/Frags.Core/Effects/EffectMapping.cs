using Frags.Core.Characters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frags.Core.Effects
{
    public class EffectMapping
    {
        public int EffectId { get; set; }
        public Effect Effect { get; set; }

        public int CharacterId { get; set; }
        public Character Character { get; set; }
    }
}

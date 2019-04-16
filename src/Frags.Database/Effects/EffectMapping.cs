using Frags.Database.Characters;

namespace Frags.Database.Effects
{
    public class EffectMapping
    {
        public CharacterDto Character { get; set; }
        public int CharacterId { get; set; }

        public EffectDto Effect { get; set; }
        public int EffectId { get; set; }
    }
}
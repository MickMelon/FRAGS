using Frags.Database.Characters;

namespace Frags.Database.Effects
{
    public class EffectMapping
    {
        public CharacterDto Character { get; set; }
        public string CharacterId { get; set; }

        public EffectDto Effect { get; set; }
        public string EffectId { get; set; }
    }
}
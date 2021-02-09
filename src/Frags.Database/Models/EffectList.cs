using Frags.Core.Characters;

namespace Frags.Database.Models
{
    public class EffectList
    {
        public EffectList(Character character)
        {
            Character = character;
            CharacterId = character.Id;
        }

        protected EffectList() { }

        public int Id { get; set; }
        public string Data { get; set; }
        public Character Character { get; set; }
        public int CharacterId { get; set; }
    }
}
using Frags.Core.Characters;
using Frags.Core.Effects;

namespace Frags.Database.Models
{
    public class StatisticList
    {
        public StatisticList(Character character)
        {
            Character = character;
            CharacterId = character.Id;
        }

        public StatisticList(Effect effect)
        {
            Effect = effect;
            EffectId = effect.Id;
        }

        protected StatisticList() { }

        public int Id { get; set; }
        
        public string Data { get; set; }
        public Character Character { get; set; }
        public int? CharacterId { get; set; }
        public Effect Effect { get; set; }
        public int? EffectId { get; set; }
    }
}
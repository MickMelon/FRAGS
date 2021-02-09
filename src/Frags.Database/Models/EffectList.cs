using Frags.Core.Characters;

namespace Frags.Database.Models
{
    public class EffectList
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public Character Character { get; set; }
    }
}
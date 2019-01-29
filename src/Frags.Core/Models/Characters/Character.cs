namespace Frags.Core.Models.Characters
{
    public class Character
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Story { get; set; }
        
        public int Experience { get; set; }
    }
}
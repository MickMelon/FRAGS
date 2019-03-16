namespace Frags.Core.Statistics
{
    public class StatisticOptions
    {
        public int InitialSetupMaxLevel { get; set; } = 1;

        public int InitialSkillPoints { get; set; }
        public int InitialSkillMax { get; set; }
        public int InitialSkillMin { get; set; }
        public int InitialSkillsAtMax { get; set; }
        public int InitialSkillsProficient { get; set; }

        public int InitialAttributePoints { get; set; }
        public int InitialAttributeMax { get; set; }
        public int InitialAttributeMin { get; set; }
        public int InitialAttributesAtMax { get; set; }
        public int InitialAttributesProficient { get; set; }
    }
}
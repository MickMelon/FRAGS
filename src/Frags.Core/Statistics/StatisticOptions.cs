namespace Frags.Core.Statistics
{
    public class StatisticOptions
    {
        public string ProgressionStrategy { get; set; }

        #region Initial Setup
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
        #endregion

        public int SkillPointsOnLevelUp { get; set; }
        public int AttributePointsOnLevelUp { get; set; }

        public int ExpMessageLengthDivisor { get; set; }
        public ulong[] ExpEnabledChannels { get; set; }
    }
}
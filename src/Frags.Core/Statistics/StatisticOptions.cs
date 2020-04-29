namespace Frags.Core.Statistics
{
    public class StatisticOptions
    {
        public int Id { get; set; }

        public string ProgressionStrategy { get; set; }

        public int AttributeMax { get; set; }
        public int SkillMax { get; set; }

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

        public double ProficientAttributeMultiplier { get; set; }
        public double ProficientSkillMultiplier { get; set; }

        public int ExpMessageLengthDivisor { get; set; }

        // Only used by the configuration JSON, i.e. not Campaigns (they use the Channel object instead.)
        public ulong[] ExpEnabledChannels { get; set; }
    }
}
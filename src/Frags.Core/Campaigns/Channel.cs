using System;
using System.Collections.Generic;
using System.Text;

namespace Frags.Core.Campaigns
{
    public class Channel
    {
        private Channel() { }
        public Channel(ulong id, Campaign campaign = null, bool isExperienceEnabled = false)
        {
            Id = id;
            Campaign = campaign;
            IsExperienceEnabled = isExperienceEnabled;
        }

        public ulong Id { get; set; }

        public Campaign Campaign { get; set; }
        public int CampaignId { get; set; }

        public bool IsExperienceEnabled { get; set; }
    }
}

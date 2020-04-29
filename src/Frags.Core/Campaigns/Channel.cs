using System;
using System.Collections.Generic;
using System.Text;

namespace Frags.Core.Campaigns
{
    public class Channel
    {
        private Channel() { }
        public Channel(ulong channelId, Campaign campaign = null, bool expEnabled = false)
        {
            Id = channelId;
            Campaign = campaign;
            IsExperienceEnabled = expEnabled;
        }

        public ulong Id { get; set; }

        public Campaign Campaign { get; set; }

        public bool IsExperienceEnabled { get; set; }
    }
}

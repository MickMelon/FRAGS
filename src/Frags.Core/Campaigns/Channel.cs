using System;
using System.Collections.Generic;
using System.Text;

namespace Frags.Core.Campaigns
{
    public class Channel
    {
        private Channel() { }
        public Channel(ulong channelId, Campaign campaign = null)
        {
            Id = channelId;
            Campaign = campaign;
        }

        public ulong Id { get; set; }

        public Campaign Campaign { get; set; }
    }
}

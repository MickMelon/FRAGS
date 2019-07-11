using System;
using System.Collections.Generic;
using System.Text;

namespace Frags.Database.Campaigns
{
    public class ChannelDto
    {
        public ulong Id { get; set; }

        public CampaignDto Campaign { get; set; }
    }
}

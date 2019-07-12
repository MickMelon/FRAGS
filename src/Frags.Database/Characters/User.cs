using Frags.Core.Common;
using Frags.Database.Campaigns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Frags.Database.Characters
{
    /// <summary>
    /// Keeps track of active characters in the database.
    /// </summary>
    public class User : BaseModel
    {
        public User()
        {
            ModeratedCampaigns = new List<Moderator>();
        }

        public int Id { get; set; }

        public ulong UserIdentifier { get; set; }

        public CharacterDto ActiveCharacter { get; set; }
        public ICollection<Moderator> ModeratedCampaigns { get; set; }
    }
}
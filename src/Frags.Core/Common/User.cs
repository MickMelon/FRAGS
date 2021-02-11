using Frags.Core.Campaigns;
using Frags.Core.Characters;
using System.Collections.Generic;

namespace Frags.Core.Common
{
    /// <summary>
    /// Keeps track of active characters in the database.
    /// </summary>
    public class User
    {
        public User(ulong userIdentifier, Character activeCharacter = null)
        {
            UserIdentifier = userIdentifier;
            ActiveCharacter = activeCharacter;
            Characters = new List<Character>();
            ModeratedCampaigns = new List<Moderator>();

            if (activeCharacter != null)
                Characters.Add(activeCharacter);
        }

        public int Id { get; private set; }

        public ulong UserIdentifier { get; set; }

        public List<Character> Characters { get; set; }
        public List<Moderator> ModeratedCampaigns { get; set; }

        public Character ActiveCharacter { get; set; }

        protected User() { }
    }
}
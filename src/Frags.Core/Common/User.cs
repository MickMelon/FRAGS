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

            if (activeCharacter != null)
                Characters.Add(activeCharacter);
        }

        public int Id { get; private set; }

        public ulong UserIdentifier { get; set; }

        public ICollection<Character> Characters { get; set; }

        public Character ActiveCharacter { get; set; }

        public static bool operator == (User user1, User user2)
        {
            return Equals(user1, user2);
        }

        public static bool operator != (User user1, User user2)
        {
            return !Equals(user1, user2);
        }

        public override bool Equals(object obj)
        {   
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this.UserIdentifier == ((User)obj).UserIdentifier;
        }
        
        public override int GetHashCode()
        {
            return (int)UserIdentifier;
        }
    }
}
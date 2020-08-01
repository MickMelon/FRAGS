using System;

namespace Frags.Core.Common.Exceptions
{
    public class CampaignException : Exception
    {
        public CampaignException(string message) : base(message)
        {
        }
    }
}
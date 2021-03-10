using System.Collections.Generic;
using System.Threading.Tasks;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Effects;
using Frags.Core.Game.Progression;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;

namespace Frags.Core.DataAccess
{
    public interface ICampaignProvider
    {
        /// <summary>
        /// Searches the database for a campaign with the matching name.
        /// </summary>
        /// <param name="campaignName">The name to search the database for.</param>
        Task<Campaign> GetCampaignAsync(string campaignName);
        
        /// <summary>
        /// Searches the database for a campaign with the given chat channel added to it.
        /// </summary>
        /// <param name="channelId">The channel identifier to search the database with.</param>
        Task<Campaign> GetCampaignAsync(ulong channelId);
        
        /// <summary>
        /// Retrieves a list of Moderators from the given campaign if it exists.
        /// A Moderator has the same control over the campaign as the owner.
        /// </summary>
        /// <param name="campaign">The campaign to retrieve a list of moderators from.</param>
        Task<List<Moderator>> GetModeratorsAsync(Campaign campaign);

        /// <summary>
        /// Replaces the list of moderators of the given campaign with a new one.
        /// </summary>
        /// <param name="campaign">The campaign to edit the moderator list of.</param>
        /// <param name="moderators">The new list of moderators for the campaign.</param>
        Task UpdateModeratorsAsync(Campaign campaign, List<Moderator> moderators);

        /// <summary>
        /// Retrieves <see cref="StatisticOptions"/> from the given campaign if it exists.
        /// The database is not guaranteed to retrieve a full Campaign object with related data,
        /// so use this method to reliably get the campaign's <see cref="StatisticOptions"/>.
        /// </summary>
        /// <param name="campaign">The campaign to retrieve <see cref="StatisticOptions"/> from.</param>
        Task<StatisticOptions> GetStatisticOptionsAsync(Campaign campaign);

        /// <summary>
        /// Replaces the <see cref="StatisticOptions"/> of the given campaign if it exists.
        /// </summary>
        /// <param name="campaign">The campaign to overwrite data in.</param>
        /// <param name="statisticOptions">The new object to overwrite the existing one.</param>
        Task UpdateStatisticOptionsAsync(Campaign campaign, StatisticOptions statisticOptions);

        /// <summary>
        /// Retrieves <see cref="RollOptions"/> from the given campaign if it exists.
        /// The database is not guaranteed to retrieve a full Campaign object with related data,
        /// so use this method to reliably get the campaign's <see cref="RollOptions"/>.
        /// </summary>
        /// <param name="campaign">The campaign to retrieve <see cref="RollOptions"/> from.</param>
        Task<RollOptions> GetRollOptionsAsync(Campaign campaign);

        /// <summary>
        /// Retrieves <see cref="IProgressionStrategy"/> from the given campaign if it exists.
        /// The database is not guaranteed to retrieve a full Campaign object with related data,
        /// so use this method to reliably get the campaign's <see cref="IProgressionStrategy"/>.
        /// </summary>
        /// <param name="campaign">The campaign to retrieve <see cref="IProgressionStrategy"/> from.</param>
        Task<IProgressionStrategy> GetProgressionStrategy(Campaign campaign);

        /// <summary>
        /// Retrieves <see cref="IRollStrategy"/> from the given campaign if it exists.
        /// The database is not guaranteed to retrieve a full Campaign object with related data,
        /// so use this method to reliably get the campaign's <see cref="IRollStrategy"/>.
        /// </summary>
        /// <param name="campaign">The campaign to retrieve <see cref="IRollStrategy"/> from.</param>
        Task<IRollStrategy> GetRollStrategy(Campaign campaign);

        /// <summary>
        /// Replaces the <see cref="RollOptions"/> of the given campaign if it exists.
        /// </summary>
        /// <param name="campaign">The campaign to overwrite data in.</param>
        /// <param name="rollOptions">The new object to overwrite the existing one.</param>
        Task UpdateRollOptionsAsync(Campaign campaign, RollOptions rollOptions);

        /// <summary>
        /// Renames the given campaign if it exists.
        /// </summary>
        /// <param name="campaign">The campaign to rename.</param>
        /// <param name="newName">The new name for the campaign.</param>
        Task RenameCampaignAsync(Campaign campaign, string newName);

        /// <summary>
        /// Retrieves a List of <see cref="Channel"/> from the given campaign if it exists.
        /// The database is not guaranteed to retrieve a full Campaign object with related data,
        /// so use this method to reliably get the campaign's list of <see cref="Channel"/>'s.
        /// </summary>
        /// <param name="campaign">The campaign to retrieve <see cref="Channel"/>'s from.</param>
        Task<List<Channel>> GetChannelsAsync(Campaign campaign);

        /// <summary>
        /// Replaces the list of channels of the given campaign with a new one.
        /// </summary>
        /// <param name="campaign">The campaign to edit the channel list of.</param>
        /// <param name="moderators">The new list of channels for the campaign.</param>
        Task UpdateChannelsAsync(Campaign campaign, List<Channel> channels);

        /// <summary>
        /// Checks the given user ID to see if they are the owner of the Campaign or a Moderator.
        /// </summary>
        /// <param name="campaign">The campaign to check if the user has permission in it.</param>
        /// <param name="userIdentifier">The user to check whether they are the owner or a Moderator.</param>
        Task<bool> HasPermissionAsync(Campaign campaign, ulong userIdentifier);

        /// <summary>
        /// Creates a new Campaign.
        /// This method should fail if another campaign is already assigned to the given channel, or if the name is not unique.
        /// </summary>
        /// <param name="userIdentifier">The owner of the new campaign. They will have full permissions over the new campaign.</param>
        /// <param name="name">The name of the new campaign. It should be unique to the database.</param>
        /// <param name="channelId">The first (of potentially many) channels to be assigned to the new campaign. A channel should only be assigned to one campaign.</param>
        Task CreateCampaignAsync(ulong userIdentifier, string name, ulong channelId);

        // TODO: Decide whether deletion should cause a cascade of any objects associated with the campaign, i.e. kill all Effects, Characters, Channels, etc...
        /// <summary>
        /// Deletes a Campaign from the database.
        /// </summary>
        /// <param name="campaign">The campaign to delete.</param>
        Task DeleteCampaignAsync(Campaign campaign);

        /// <summary>
        /// Adds a new channel to the given campaign.
        /// This method should fail if another campaign is already assigned to the given channel (including the given campaign.)
        /// </summary>
        /// <param name="campaign">The campaign to add the channel to.</param>
        /// <param name="channelId">The new channel to be assigned to the campaign. A channel should only be assigned to one campaign.</param>
        Task SetCampaignChannelAsync(Campaign campaign, ulong channelId);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Common.Exceptions;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Progression;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfCampaignProvider : ICampaignProvider
    {
        private readonly RpgContext _context;
        private readonly List<IProgressionStrategy> _progStrategies;
        private readonly List<IRollStrategy> _rollStrategies;

        public EfCampaignProvider(RpgContext context,
        List<IProgressionStrategy> progStrategies,
        List<IRollStrategy> rollStrategies)
        {
            _context = context;
            _progStrategies = progStrategies;
            _rollStrategies = rollStrategies;
        }

        public async Task CreateCampaignAsync(ulong userIdentifier, string name)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userIdentifier);
            if (user == null)
            {
                user = new User(userIdentifier);
                await _context.AddAsync(user);
            }

            Campaign newCamp = new Campaign(user,name);
            await _context.AddAsync(newCamp);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCampaignAsync(Campaign campaign)
        {
            Campaign camp = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            _context.Remove(camp);
            await _context.SaveChangesAsync();
        }

        public async Task<Campaign> GetCampaignAsync(string campaignName)
        {
            return await _context.Campaigns.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(campaignName));
        }

        public async Task<Campaign> GetCampaignAsync(ulong channelId)
        {
            Channel channel = await _context.Set<Channel>().FirstOrDefaultAsync(x => x.Id == channelId);
            if (channel == null) return null;
            await _context.Entry(channel).Reference(x => x.Campaign).LoadAsync();
            _context.Entry(channel).State = EntityState.Detached;

            Campaign camp = channel.Campaign;
            if (camp == null) return null;

            await _context.Entry(camp).Reference(x => x.Owner).LoadAsync();
            _context.Entry(camp.Owner).State = EntityState.Detached;
            _context.Entry(camp).State = EntityState.Detached;
            
            return camp;
        }

        public async Task<List<Channel>> GetChannelsAsync(Campaign campaign)
        {
            Campaign camp = await _context.Campaigns.Include(x => x.Channels).AsNoTracking().FirstOrDefaultAsync(x => x.Id == campaign.Id);
            List<Channel> channels = camp?.Channels;

            return channels;
        }

        public async Task<List<Character>> GetCharactersAsync(Campaign campaign)
        {
            Campaign camp = await _context.Campaigns.Include(x => x.Characters).AsNoTracking().FirstOrDefaultAsync(x => x.Id == campaign.Id);
            List<Character> characters = camp?.Characters;

            return characters;
        }

        public async Task<List<Moderator>> GetModeratorsAsync(Campaign campaign)
        {
            Campaign camp = await _context.Campaigns.Include(x => x.ModeratedCampaigns).AsNoTracking().FirstOrDefaultAsync(x => x.Id == campaign.Id);
            List<Moderator> mods = camp?.ModeratedCampaigns;

            return mods;
        }

        public async Task<StatisticOptions> GetStatisticOptionsAsync(Campaign campaign)
        {
            Campaign camp = await _context.Campaigns.Include(x => x.StatisticOptions).AsNoTracking().FirstOrDefaultAsync(x => x.Id == campaign.Id);
            StatisticOptions statOpts = camp?.StatisticOptions;

            return statOpts;
        }

        public async Task<bool> HasPermissionAsync(Campaign campaign, ulong userIdentifier)
        {
            if (campaign == null) return false;
            if (campaign.Owner?.UserIdentifier == userIdentifier) return true;
            
            List<Moderator> moderators = await GetModeratorsAsync(campaign);
            if (moderators == null) return false;
            
            return moderators.Select(x => x.User.UserIdentifier).Contains(userIdentifier);
        }

        public async Task RenameCampaignAsync(Campaign campaign, string newName)
        {
            Campaign camp = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            camp.Name = newName;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateModeratorsAsync(Campaign campaign, List<Moderator> moderators)
        {
            Campaign camp = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            camp.ModeratedCampaigns = moderators;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatisticOptionsAsync(Campaign campaign, StatisticOptions statisticOptions)
        {
            Campaign camp = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            camp.StatisticOptions = statisticOptions;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateChannelsAsync(Campaign campaign, List<Channel> channels)
        {
            Campaign camp = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            camp.Channels = channels;
            await _context.SaveChangesAsync();
        }

        public async Task SetCampaignChannelAsync(Campaign campaign, ulong channelId)
        {
            Channel channel = await _context.Set<Channel>().FirstOrDefaultAsync(x => x.Id == channelId);

            if (channel == null)
            {
                await _context.AddAsync(new Channel(channelId, campaign));
            }
            else
            {
                channel.CampaignId = campaign?.Id ?? 0;
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task<RollOptions> GetRollOptionsAsync(Campaign campaign)
        {
            Campaign camp = await _context.Campaigns.Include(x => x.RollOptions).AsNoTracking().FirstOrDefaultAsync(x => x.Id == campaign.Id);
            RollOptions rollOpts = camp?.RollOptions;

            return rollOpts;
        }

        public async Task UpdateRollOptionsAsync(Campaign campaign, RollOptions rollOptions)
        {
            Campaign camp = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            camp.RollOptions = rollOptions;
            await _context.SaveChangesAsync();
        }

        public async Task<IProgressionStrategy> GetProgressionStrategy(Campaign campaign)
        {
            StatisticOptions statOpts = await GetStatisticOptionsAsync(campaign);
            return _progStrategies?.Find(x => x.GetType().Name.ContainsIgnoreCase(statOpts.ProgressionStrategy));
        }

        public async Task<IRollStrategy> GetRollStrategy(Campaign campaign)
        {
            RollOptions rollOpts = await GetRollOptionsAsync(campaign);
            return _rollStrategies?.Find(x => x.GetType().Name.ContainsIgnoreCase(rollOpts.RollStrategy));
        }
    }
}
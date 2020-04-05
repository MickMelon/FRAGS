using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Frags.Database.Campaigns;
using Frags.Database.Characters;
using Frags.Database.Effects;
using Frags.Database.Statistics;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfCampaignProvider : ICampaignProvider
    {
        private readonly RpgContext _context;

        private readonly IMapper _mapper;

        private readonly IUserProvider _userProvider;

        public EfCampaignProvider(RpgContext context, IMapper mapper, IUserProvider userProvider)
        {
            _context = context;
            _mapper = mapper;
            _userProvider = userProvider;
        }

        public async Task<Campaign> CreateCampaignAsync(ulong userIdentifier, string name)
        {
            var userDto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userIdentifier);

            if (userDto == null)
            {
                await _userProvider.CreateUserAsync(userIdentifier);
            }

            // create campaign object first, add the user later to avoid circular dependency
            var campDto = new CampaignDto();

            await _context.AddAsync(campDto);
            await _context.SaveChangesAsync();

            campDto.Name = name;
            campDto.Owner = userDto;
            await _context.SaveChangesAsync();

            return _mapper.Map<Campaign>(campDto);
        }

        public async Task<bool> DeleteCampaignAsync(Campaign campaign)
        {
            var dto = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);
            if (dto == null) return false;

            _context.Remove(dto);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Campaign> GetCampaignAsync(int id)
        {
            return _mapper.Map<Campaign>(await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == id));
        }

        public async Task<Campaign> GetCampaignAsync(string name)
        {
            return _mapper.Map<Campaign>(await _context.Campaigns.FirstOrDefaultAsync(x => x.Name.Equals(name)));
        }

        public async Task<Campaign> GetCampaignFromChannelAsync(ulong channelId)
        {
            var channel = await _context.Set<ChannelDto>().Where(x => x.Id == channelId).Include(y => y.Campaign).FirstOrDefaultAsync();

            return _mapper.Map<Campaign>(channel?.Campaign);
        }

        public async Task UpdateCampaignAsync(Campaign campaign)
        {
            // If the campaign does not exist in the database, abort
            if (await _context.Campaigns.CountAsync(c => c.Id.Equals(campaign.Id)) <= 0)
                return;

            // Load related properties and get ready to manually map certain navigation props
            var campDto = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);
            _mapper.Map(campaign, campDto);

            // Manually map Exp enabled channels
            var channelsToMap = campaign.StatisticOptions?.ExpEnabledChannels;
            if (channelsToMap != null)
            {
                campDto.StatisticOptions.ExpEnabledChannels = new List<ChannelDto>();
                foreach (var channelId in channelsToMap)
                {
                    campDto.StatisticOptions.ExpEnabledChannels.Add(new ChannelDto { Campaign = campDto, Id = channelId });
                }
            }

            _context.Update(campDto);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetModeratorsAsync(int id)
        {
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<List<User>>(await _context.Entry(campaign).Collection(x => x.ModeratedCampaigns).Query().Select(x => x.User).ToListAsync());
        }

        public async Task<ICollection<Channel>> GetChannelsAsync(int id)
        {
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<List<Channel>>(await _context.Entry(campaign).Collection(x => x.Channels).Query().ToListAsync());
        }

        public async Task<ICollection<Character>> GetCharactersAsync(int id)
        {
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == id);

            // TODO: make an extension method to do all this stuff for you
            var characterDtos = await _context.Entry(campaign).Collection(x => x.Characters).Query()
                .Include(x => x.User)
                .Include(x => x.Statistics).ThenInclude(x => x.Statistic)
                .Include(x => x.Statistics).ThenInclude(x => x.StatisticValue)
                .Include(x => x.EffectMappings).ThenInclude(x => x.Effect).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.Statistic)
                .Include(x => x.EffectMappings).ThenInclude(x => x.Effect).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.StatisticValue)
                .ToListAsync();

            return _mapper.Map<List<Character>>(characterDtos);
        }

        public async Task<ICollection<Effect>> GetEffectsAsync(int id)
        {
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<List<EffectDto>, List<Effect>>(await _context.Entry(campaign).Collection(x => x.Effects).Query().ToListAsync());
        }

        public async Task<ICollection<Statistic>> GetStatisticsAsync(int id)
        {
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<List<StatisticDto>, List<Statistic>>(await _context.Entry(campaign).Collection(x => x.Statistics).Query().ToListAsync());
        }

        public async Task<RollOptions> GetRollOptionsAsync(int id)
        {
            var campaign = await _context.Campaigns.Where(x => x.Id == id).Include(y => y.RollOptions).FirstOrDefaultAsync();
            return _mapper.Map<RollOptions>(campaign?.RollOptions);
        }

        public async Task<StatisticOptions> GetStatisticOptionsAsync(int id)
        {
            var campaign = await _context.Campaigns.Where(x => x.Id == id).Include(y => y.StatisticOptions).FirstOrDefaultAsync();
            if (campaign == null) return null;

            var mapped = _mapper.Map<StatisticOptions>(campaign.StatisticOptions);
            mapped.ExpEnabledChannels = campaign.StatisticOptions.ExpEnabledChannels.Select(x => x.Id).ToArray();
            return mapped;
        }

        /// <inheritdoc/>
        public async Task<List<Campaign>> GetOwnedCampaignsAsync(ulong userIdentifier)
        {
            var user = await _userProvider.GetUserAsync(userIdentifier);
            if(user == null) return null;

            return _mapper.Map<List<Campaign>>(await _context.Campaigns.Where(x => x.Owner.UserIdentifier == userIdentifier).ToListAsync());
        }
    }
}
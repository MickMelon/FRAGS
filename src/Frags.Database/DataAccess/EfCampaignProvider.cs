using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Common.Exceptions;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Progression;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Frags.Database.Campaigns;
using Frags.Database.Characters;
using Frags.Database.Statistics;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfCampaignProvider : ICampaignProvider
    {
        private readonly RpgContext _context;
        private readonly IMapper _mapper;
        private readonly List<IProgressionStrategy> _progStrategies;
        private readonly List<IRollStrategy> _rollStrategies;

        public EfCampaignProvider(RpgContext context,
        IMapper mapper,
        List<IProgressionStrategy> progStrategies,
        List<IRollStrategy> rollStrategies)
        {
            _context = context;
            _mapper = mapper;
            _progStrategies = progStrategies;
            _rollStrategies = rollStrategies;
        }

        public async Task CreateCampaignAsync(ulong userIdentifier, string name)
        {
            UserDto userDto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userIdentifier);
            if (userDto == null)
            {
                userDto = new UserDto(userIdentifier);
                await _context.AddAsync(userDto);
            }

            CampaignDto newCamp = new CampaignDto { Owner = userDto, Name = name };
            await _context.AddAsync(newCamp);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCampaignAsync(Campaign campaign)
        {
            CampaignDto dto = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            _context.Remove(dto);
            await _context.SaveChangesAsync();
        }

        public async Task<Campaign> GetCampaignAsync(string campaignName)
        {
            return _mapper.Map<Campaign>(await _context.Campaigns.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(campaignName)));
        }

        public async Task<Campaign> GetCampaignAsync(ulong channelId)
        {
            ChannelDto channelDto = await _context.Set<ChannelDto>().FirstOrDefaultAsync(x => x.Id == channelId);
            if (channelDto == null) return null;
            await _context.Entry(channelDto).Reference(x => x.Campaign).LoadAsync();
            _context.Entry(channelDto).State = EntityState.Detached;

            CampaignDto campDto = channelDto.Campaign;
            if (campDto == null) return null;

            await _context.Entry(campDto).Reference(x => x.Owner).LoadAsync();
            _context.Entry(campDto.Owner).State = EntityState.Detached;
            _context.Entry(campDto).State = EntityState.Detached;
            
            return _mapper.Map<Campaign>(campDto);
        }

        public async Task<List<Channel>> GetChannelsAsync(Campaign campaign)
        {
            CampaignDto campDto = await _context.Campaigns.Include(x => x.Channels).AsNoTracking().FirstOrDefaultAsync(x => x.Id == campaign.Id);
            List<ChannelDto> channelDtos = campDto?.Channels;

            return _mapper.Map<List<Channel>>(channelDtos);
        }

        public async Task<List<Character>> GetCharactersAsync(Campaign campaign)
        {
            CampaignDto campDto = await _context.Campaigns.Include(x => x.Characters).AsNoTracking().FirstOrDefaultAsync(x => x.Id == campaign.Id);
            List<CharacterDto> charDtos = campDto?.Characters;

            return _mapper.Map<List<Character>>(charDtos);
        }

        public async Task<List<Moderator>> GetModeratorsAsync(Campaign campaign)
        {
            CampaignDto campDto = await _context.Campaigns.Include(x => x.ModeratedCampaigns).AsNoTracking().FirstOrDefaultAsync(x => x.Id == campaign.Id);
            List<ModeratorDto> modDtos = campDto?.ModeratedCampaigns;

            return _mapper.Map<List<Moderator>>(modDtos);
        }

        public async Task<StatisticOptions> GetStatisticOptionsAsync(Campaign campaign)
        {
            CampaignDto campDto = await _context.Campaigns.Include(x => x.StatisticOptions).AsNoTracking().FirstOrDefaultAsync(x => x.Id == campaign.Id);
            StatisticOptionsDto optDto = campDto?.StatisticOptions;

            return _mapper.Map<StatisticOptions>(optDto);
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
            CampaignDto dto = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            dto.Name = newName;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateModeratorsAsync(Campaign campaign, List<Moderator> moderators)
        {
            CampaignDto dto = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            dto.ModeratedCampaigns = _mapper.Map<List<ModeratorDto>>(moderators);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatisticOptionsAsync(Campaign campaign, StatisticOptions statisticOptions)
        {
            CampaignDto dto = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            dto.StatisticOptions = _mapper.Map<StatisticOptionsDto>(statisticOptions);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateChannelsAsync(Campaign campaign, List<Channel> channels)
        {
            CampaignDto dto = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            dto.Channels = _mapper.Map<List<ChannelDto>>(channels);
            await _context.SaveChangesAsync();
        }

        public async Task SetCampaignChannelAsync(Campaign campaign, ulong channelId)
        {
            ChannelDto channel = await _context.Set<ChannelDto>().FirstOrDefaultAsync(x => x.Id == channelId);

            if (channel == null)
            {
                await _context.AddAsync(new ChannelDto { Id = channelId, CampaignId = campaign.Id } );
            }
            else
            {
                channel.CampaignId = campaign?.Id ?? 0;
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task<RollOptions> GetRollOptionsAsync(Campaign campaign)
        {
            CampaignDto campDto = await _context.Campaigns.Include(x => x.RollOptions).AsNoTracking().FirstOrDefaultAsync(x => x.Id == campaign.Id);
            RollOptionsDto rollDto = campDto?.RollOptions;

            return _mapper.Map<RollOptions>(rollDto);
        }

        public async Task UpdateRollOptionsAsync(Campaign campaign, RollOptions rollOptions)
        {
            CampaignDto dto = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            dto.RollOptions = _mapper.Map<RollOptionsDto>(rollOptions);
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
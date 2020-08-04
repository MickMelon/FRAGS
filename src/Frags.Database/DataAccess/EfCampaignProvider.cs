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

        public EfCampaignProvider(RpgContext context,
        IMapper mapper,
        List<IProgressionStrategy> progStrategies)
        {
            _context = context;
            _mapper = mapper;
            _progStrategies = progStrategies;
        }

        public async Task AddChannelAsync(string campaignName, ulong channelId)
        {
            CampaignDto campDto = await _context.Campaigns.FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(campaignName));
            if (campDto == null) throw new CampaignException(Messages.CAMP_NOT_FOUND_NAME);

            await _context.Entry(campDto).Collection(x => x.Channels).LoadAsync();
            var campChannels = campDto.Channels;
            if (campChannels == null) campChannels = new List<ChannelDto>();

            if (campChannels.Any(x => x.Id == channelId))
                throw new CampaignException(Messages.CAMP_CHANNEL_ALREADY_ADDED);

            var chanDto = new ChannelDto { Id = channelId, Campaign = campDto };

            await _context.AddAsync(chanDto);
            await _context.SaveChangesAsync();
        }

        public async Task ConfigureCampaignAsync(ulong callerId, ulong channelId, string propName, object value)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == callerId);
            if (user == null) throw new CampaignException("User does not exist and therefore does not moderate or own any Campaigns.");

            var campaignDto = (await _context.Set<ChannelDto>().Include(x => x.Campaign).FirstOrDefaultAsync(x => x.Id == channelId))?.Campaign;
            if (campaignDto == null) throw new CampaignException("Channel not associated with a Campaign.");

            var moderators = await _context.Entry(campaignDto).Collection(x => x.ModeratedCampaigns).Query().ToListAsync();

            // Caller is a moderator or owner of this campaign
            if (campaignDto.Owner.Id == user.Id || (moderators != null && moderators.Any(x => x.UserId == user.Id)))
            {
                await _context.Entry(campaignDto).Reference(x => x.StatisticOptions).LoadAsync();

                if (campaignDto.StatisticOptions == null) campaignDto.StatisticOptions = new StatisticOptionsDto();

                var propertyInfo = campaignDto.StatisticOptions.GetType().GetProperty(propName);
                if (propertyInfo == null || propertyInfo.Name == "Id" || propertyInfo.Name == "ExpEnabledChannels") 
                    throw new CampaignException("Invalid setting name!");

                var propertyType = propertyInfo.PropertyType;
                value = Convert.ChangeType(value, propertyType);

                try
                {
                    propertyInfo.SetValue(campaignDto.StatisticOptions, value);
                }
                catch (System.Exception e)
                {
                    throw new CampaignException(e.Message);
                }
                
                _context.Update(campaignDto);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new CampaignException("You don't have permission to do that.");
            }
        }

        // This should be in Controller, but we get errors from Entity Framework (has to do with entities being loaded multiple times).
        public async Task ConvertCharacterAsync(ulong callerId, ulong channelId)
        {
            var userDto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == callerId);
            if (userDto == null) throw new CampaignException(Messages.USER_NOT_FOUND);

            // Load active character
            await _context.Entry(userDto).Reference(x => x.ActiveCharacter).LoadAsync();
            var charDto = userDto.ActiveCharacter;
            if (charDto == null) throw new CampaignException(Messages.CHAR_NOT_FOUND);

            // Since we want the DTO, we can't just use GetCampaignAsync
            var campDto = (await _context.Set<ChannelDto>().Include(x => x.Campaign).FirstOrDefaultAsync(x => x.Id == channelId)).Campaign;
            if (campDto == null) throw new CampaignException(Messages.CAMP_NOT_FOUND_CHANNEL);

            // Load StatisticOptions
            await _context.Entry(campDto).Reference(x => x.StatisticOptions).LoadAsync();

            var statOptions = campDto.StatisticOptions;
            if (statOptions == null) throw new CampaignException(Messages.CAMP_STATOPTIONS_NOT_FOUND);

            var strategy = GetProgressionStrategy(statOptions);
            if (strategy == null) throw new CampaignException(Messages.CAMP_PROGSTRATEGY_INVALID);

            var mappedChar = _mapper.Map<Character>(charDto);
            await strategy.ResetCharacter(mappedChar);
            _mapper.Map(mappedChar, charDto);
            
            charDto.Campaign = campDto;
            charDto.CampaignId = campDto.Id;

            _context.Update(charDto);
            await _context.SaveChangesAsync();
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

        public Task DeleteCampaignAsync(string campaignName)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Campaign> GetCampaignAsync(string campaignName)
        {
            return _mapper.Map<Campaign>(await _context.Campaigns.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(campaignName)));
        }

        public async Task<Campaign> GetCampaignAsync(ulong channelId)
        {
            CampaignDto campDto = (await _context.Set<ChannelDto>().Include(x => x.Campaign).ThenInclude(x => x.Owner).FirstOrDefaultAsync(x => x.Id == channelId))?.Campaign;

            return _mapper.Map<Campaign>(campDto);
        }

        public async Task<List<Channel>> GetChannelsAsync(Campaign campaign)
        {
            CampaignDto campDto = await _context.Campaigns.Include(x => x.Channels).FirstOrDefaultAsync(x => x.Id == campaign.Id);
            List<ChannelDto> channelDtos = campDto?.Channels;

            return _mapper.Map<List<Channel>>(channelDtos);
        }

        public async Task<List<Character>> GetCharactersAsync(Campaign campaign)
        {
            CampaignDto campDto = await _context.Campaigns.Include(x => x.Characters).FirstOrDefaultAsync(x => x.Id == campaign.Id);
            List<CharacterDto> charDtos = campDto?.Characters;

            return _mapper.Map<List<Character>>(charDtos);
        }

        public async Task<List<Moderator>> GetModeratorsAsync(Campaign campaign)
        {
            CampaignDto campDto = await _context.Campaigns.Include(x => x.ModeratedCampaigns).FirstOrDefaultAsync(x => x.Id == campaign.Id);
            List<ModeratorDto> modDtos = campDto?.ModeratedCampaigns;

            return _mapper.Map<List<Moderator>>(modDtos);
        }

        public async Task<StatisticOptions> GetStatisticOptionsAsync(Campaign campaign)
        {
            CampaignDto campDto = await _context.Campaigns.Include(x => x.StatisticOptions).FirstOrDefaultAsync(x => x.Id == campaign.Id);
            StatisticOptionsDto optDto = campDto?.StatisticOptions;

            return _mapper.Map<StatisticOptions>(optDto);
        }

        public async Task RenameCampaignAsync(ulong callerId, string newName, ulong channelId)
        {
            if (await _context.Campaigns.CountAsync(x => x.Name.EqualsIgnoreCase(newName)) > 0)
                throw new CampaignException(Messages.CAMP_EXISTING_NAME);

            CampaignDto dto = (await _context.Set<ChannelDto>().Include(x => x.Campaign).FirstOrDefaultAsync(x => x.Id == channelId))?.Campaign;
            if (dto == null) throw new CampaignException(Messages.CAMP_NOT_FOUND_CHANNEL);

            await _context.Entry(dto).Reference(x => x.Owner).LoadAsync();
            await _context.Entry(dto).Collection(x => x.ModeratedCampaigns).LoadAsync();
            IEnumerable<UserDto> users = dto.ModeratedCampaigns?.Select(x => x.User);

            if (dto.Owner.UserIdentifier == callerId || (users != null && users.Any(x => x.UserIdentifier == callerId)))
            {
                dto.Name = newName;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new CampaignException(Messages.CAMP_ACCESS_DENIED);
            }
        }

        public async Task UpdateCampaignAsync(Campaign campaign)
        {
            CampaignDto dto = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);

            _mapper.Map(campaign, dto);

            _context.Update(dto);
            await _context.SaveChangesAsync();
        }

        private IProgressionStrategy GetProgressionStrategy(StatisticOptionsDto options)
        {
            if (string.IsNullOrWhiteSpace(options.ProgressionStrategy))
                return null;

            return _progStrategies.Find(x => x.GetType().Name.ContainsIgnoreCase(options.ProgressionStrategy));
        }
    }
}
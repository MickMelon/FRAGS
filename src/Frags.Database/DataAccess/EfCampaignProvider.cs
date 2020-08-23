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

            // We need to see if the channel is already associated with a Campaign
            ChannelDto channelDto = await _context.Set<ChannelDto>().FirstOrDefaultAsync(x => x.Id == channelId);
            if (channelDto == null)
            {
                channelDto = new ChannelDto { Id = channelId, Campaign = campDto };
                await _context.AddAsync(channelDto);
            }
            else
            {
                if (channelDto.CampaignId > 0)
                    throw new CampaignException(Messages.CAMP_CHANNEL_ALREADY_ADDED);
                else
                    channelDto.CampaignId = campDto.Id;
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task RemoveChannelAsync(ulong channelId)
        {
            ChannelDto channelDto = await _context.Set<ChannelDto>().FirstOrDefaultAsync(x => x.Id == channelId);
            if (channelDto == null)
                throw new CampaignException(Messages.CAMP_NOT_FOUND_CHANNEL);

            channelDto.Campaign = null;
            channelDto.CampaignId = 0;

            await _context.SaveChangesAsync();
        }

        public async Task ConfigureCampaignAsync(ulong callerId, ulong channelId, string propName, object value)
        {
            var userDto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == callerId);
            if (userDto == null) throw new CampaignException(Messages.USER_NOT_FOUND);

            // Since we want the DTO, we can't just use GetCampaignAsync
            var campaignDto = (await _context.Set<ChannelDto>().Include(x => x.Campaign).FirstOrDefaultAsync(x => x.Id == channelId))?.Campaign;
            if (campaignDto == null) throw new CampaignException(Messages.CAMP_NOT_FOUND_CHANNEL);

            // Caller is a moderator or owner of this campaign
            if (await HasPermissionAsync(callerId, campaignDto))
            {
                await _context.Entry(campaignDto).Reference(x => x.StatisticOptions).LoadAsync();
                var options = campaignDto.StatisticOptions;

                if (options == null) options = new StatisticOptionsDto();

                // Try to match propName to a property in StatisticOptions
                var propertyInfo = options.GetType().GetProperty(propName);
                if (propertyInfo == null || propertyInfo.Name == nameof(options.Id) || propertyInfo.Name == nameof(options.ExpEnabledChannels)) 
                    throw new CampaignException(Messages.CAMP_PROPERTY_INVALID);

                try
                {
                    // Try to convert our given object (probably a string or int) to the same type as the property
                    var propertyType = propertyInfo.PropertyType;
                    value = Convert.ChangeType(value, propertyType);

                    propertyInfo.SetValue(options, value);
                }
                catch (System.Exception)
                {
                    throw new CampaignException(Messages.CAMP_PROPERTY_INVALID_VALUE);
                }
                
                campaignDto.StatisticOptions = options;
                _context.Update(campaignDto);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new CampaignException(Messages.CAMP_ACCESS_DENIED);
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
            var campDto = (await _context.Set<ChannelDto>().Include(x => x.Campaign).ThenInclude(x => x.StatisticOptions).FirstOrDefaultAsync(x => x.Id == channelId)).Campaign;
            if (campDto == null) throw new CampaignException(Messages.CAMP_NOT_FOUND_CHANNEL);

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

        public async Task<bool> HasPermissionAsync(ulong userIdentifier, ulong channelId)
        {
            CampaignDto campDto = (await _context.Set<ChannelDto>().Include(x => x.Campaign).ThenInclude(x => x.Owner).AsNoTracking().FirstOrDefaultAsync(x => x.Id == channelId))?.Campaign;
            return await HasPermissionAsync(userIdentifier, campDto);
        }

        public async Task<bool> HasPermissionAsync(ulong userIdentifier, string name)
        {
            CampaignDto campDto = await _context.Campaigns.Include(x => x.Owner).AsNoTracking().FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(name));
            return await HasPermissionAsync(userIdentifier, campDto);
        }

        private async Task<bool> HasPermissionAsync(ulong userIdentifier, CampaignDto campDto)
        {
            if (campDto == null) return false;
            if (campDto.Owner?.UserIdentifier == userIdentifier) return true;
            await _context.Entry(campDto).Collection(x => x.ModeratedCampaigns).LoadAsync();
            if (campDto.ModeratedCampaigns == null) return false;

            foreach (ModeratorDto modDto in campDto.ModeratedCampaigns)
                _context.Entry(modDto).State = EntityState.Detached;
            
            return campDto.ModeratedCampaigns.Select(x => x.User.UserIdentifier).Contains(userIdentifier);
        }

        public async Task RenameCampaignAsync(ulong callerId, string newName, ulong channelId)
        {
            if (await _context.Campaigns.CountAsync(x => x.Name.EqualsIgnoreCase(newName)) > 0)
                throw new CampaignException(Messages.CAMP_EXISTING_NAME);

            CampaignDto dto = (await _context.Set<ChannelDto>().Include(x => x.Campaign).ThenInclude(x => x.Owner).FirstOrDefaultAsync(x => x.Id == channelId))?.Campaign;
            if (dto == null) throw new CampaignException(Messages.CAMP_NOT_FOUND_CHANNEL);

            if (await HasPermissionAsync(callerId, dto))
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
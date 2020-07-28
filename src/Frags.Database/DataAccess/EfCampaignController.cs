using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
using Frags.Core.Common;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Effects;
using Frags.Core.Game.Progression;
using Frags.Core.Game.Rolling;
using Frags.Core.Statistics;
using Frags.Database.Campaigns;
using Frags.Database.Characters;
using Frags.Database.Effects;
using Frags.Database.Statistics;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfCampaignController : ICampaignController
    {
        private readonly RpgContext _context;

        private readonly IMapper _mapper;

        private readonly IUserProvider _userProvider;

        private readonly ICharacterProvider _charProvider;

        private readonly List<IProgressionStrategy> _progStrategies;

        public EfCampaignController(RpgContext context,
         IMapper mapper,
          IUserProvider userProvider,
           ICharacterProvider charProvider,
            List<IProgressionStrategy> progStrategies)
        {
            _context = context;
            _mapper = mapper;
            _userProvider = userProvider;
            _charProvider = charProvider;
            _progStrategies = progStrategies;
        }

        public async Task<string> AddCampaignChannelAsync(string campaignName, ulong channelId)
        {
            var campDto = await _context.Campaigns.Include(x => x.Channels).FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(campaignName));
            if (campDto == null) return "Campaign not found";

            if (campDto.Channels.Any(x => x.Id == channelId))
                return "Channel already added";

            campDto.Channels.Add(new ChannelDto { Id = channelId, Campaign = campDto });

            await _context.SaveChangesAsync();
            return "Success";
        }

        public async Task<string> ConfigureCampaignAsync(ulong callerId, ulong channelId)
        {
            var user = await _userProvider.GetUserAsync(callerId);
            if (user == null) return "User does not exist and therefore does not moderate or own any Campaigns.";

            var campaignDto = (await _context.Set<ChannelDto>().Include(x => x.Campaign).FirstOrDefaultAsync(x => x.Id == channelId))?.Campaign;
            if (campaignDto == null) return "Channel not associated with a Campaign.";

            var moderators = await _context.Entry(campaignDto).Collection(x => x.ModeratedCampaigns).Query().ToListAsync();

            // Caller is a moderator or owner of this campaign
            if (campaignDto.Owner.Id == user.Id || (moderators != null && moderators.Any(x => x.UserId == user.Id)))
            {
                campaignDto.StatisticOptions = new StatisticOptionsDto
                {
                    ProgressionStrategy = "Generic"
                };
                _context.Update(campaignDto);
                await _context.SaveChangesAsync();
                return "Done.";
            }
            else
            {
                return "You don't have permission to do that.";
            }
        }

        public async Task<string> ConvertCharacterAsync(ulong callerId, ulong channelId)
        {
            var userDto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == callerId);
            if (userDto == null) return "User not found!";

            await _context.Entry(userDto).Reference(x => x.ActiveCharacter).LoadAsync();
            if (userDto.ActiveCharacter == null) return "Character not found!";

            var campDto = (await _context.Set<ChannelDto>().Include(x => x.Campaign).FirstOrDefaultAsync(x => x.Id == channelId)).Campaign;
            if (campDto == null) return "Channel not associated with Campaign!";

            await _context.Entry(campDto).Reference(x => x.StatisticOptions).LoadAsync();

            var statOptions = campDto.StatisticOptions;
            if (statOptions == null) return "Campaign has not set its StatisticOptions!";

            var strategy = GetProgressionStrategy(statOptions);
            if (strategy == null) return "Campaign has not set its ProgressionStrategy (or it's invalid!)";

            var mapped = _mapper.Map<Character>(userDto.ActiveCharacter);
            await strategy.ResetCharacter(mapped);
            _mapper.Map(mapped, userDto.ActiveCharacter);
            
            userDto.ActiveCharacter.Campaign = campDto;
            userDto.ActiveCharacter.CampaignId = campDto.Id;

            _context.Update(userDto.ActiveCharacter);
            await _context.SaveChangesAsync();
            
            return "Success!!";
        }

        public async Task<string> CreateCampaignAsync(ulong userIdentifier, string name)
        {
            if (await _context.Campaigns.CountAsync(x => x.Name.EqualsIgnoreCase(name)) >= 1)
                return "There's already a Campaign with that name :(";

            var userDto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userIdentifier);

            if (userDto == null)
            {
                await _userProvider.CreateUserAsync(userIdentifier);
                userDto = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == userIdentifier);
            }

            var campDto = new CampaignDto();
            campDto.Name = name;
            campDto.Owner = userDto;

            await _context.AddAsync(campDto);
            await _context.SaveChangesAsync();

            return "Success";
        }

        public async Task<string> GetCampaignInfoAsync(string campaignName) =>
            await GetCampaignInfo(await _context.Campaigns.FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(campaignName)));

        public async Task<string> GetCampaignInfoAsync(ulong channelId) =>
            await GetCampaignInfo((await _context.Set<ChannelDto>().Include(x => x.Campaign).FirstOrDefaultAsync(x => x.Id == channelId)).Campaign);

        private async Task<string> GetCampaignInfo(CampaignDto campDto)
        {
            if (campDto == null) return "Campaign not found!";

            await _context.Entry(campDto).Reference(x => x.Owner).LoadAsync();
            await _context.Entry(campDto).Collection(x => x.Channels).LoadAsync();
            await _context.Entry(campDto).Collection(x => x.Characters).LoadAsync();

            StringBuilder sb = new StringBuilder();

            sb.Append($"Name: {campDto.Name}\n");
            sb.Append($"Owner: {campDto.Owner.UserIdentifier}\n");
            sb.Append($"Channels: {string.Join(" ", campDto.Channels?.Select(x => x.Id)) ?? "None"}\n");
            sb.Append($"Characters: {string.Join(" ", campDto.Characters?.Select(x => x.Name)) ?? "None"}\n");

            return sb.ToString();
        }

        private IProgressionStrategy GetProgressionStrategy(StatisticOptionsDto options) =>
            _progStrategies.Find(x => x.GetType().Name.ContainsIgnoreCase(options.ProgressionStrategy));
    }
}
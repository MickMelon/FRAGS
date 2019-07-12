using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Campaigns;
using Frags.Core.Characters;
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

        public EfCampaignProvider(RpgContext context)
        {
            _context = context;
            
            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<Campaign, CampaignDto>();
                cfg.CreateMap<CampaignDto, Campaign>();
                cfg.CreateMap<Character, CharacterDto>();
                cfg.CreateMap<CharacterDto, Character>();
                cfg.CreateMap<Effect, EffectDto>();
                cfg.CreateMap<EffectDto, Effect>();

                cfg.CreateMap<Statistic, StatisticDto>()
                    .Include<Attribute, AttributeDto>()
                    .Include<Skill, SkillDto>();

                cfg.CreateMap<StatisticDto, Statistic>()
                    .Include<AttributeDto, Attribute>()
                    .Include<SkillDto, Skill>();

                cfg.CreateMap<Skill, SkillDto>();
                cfg.CreateMap<SkillDto, Skill>();

                cfg.CreateMap<Attribute, AttributeDto>();
                cfg.CreateMap<AttributeDto, Attribute>();
            });

            _mapper = new Mapper(mapperConfig);
        }

        private async Task<Campaign> CreateCampaignAsync(Campaign campaign)
        {
            var campDto = _mapper.Map<CampaignDto>(campaign);

            // Check the database for a character with the same ID as the new one
            // If one exists, don't add it
            if (await _context.Campaigns.CountAsync(x => x.Id.Equals(campDto.Id)) > 0)
                return null;

            await _context.AddAsync(campDto);
            
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserIdentifier == campaign.OwnerUserIdentifier);

            if (user == null)
            {
                user = new User { UserIdentifier = campaign.OwnerUserIdentifier };
                await _context.AddAsync(user);
            }

            var moderator = new Moderator { Campaign = campDto, User = user };
            user.ModeratedCampaigns.Add(moderator);
            campDto.Moderators.Add(moderator);

            await _context.SaveChangesAsync();
            return _mapper.Map<Campaign>(campDto);
        }

        /// <inheritdoc/>
        public async Task<Campaign> CreateCampaignAsync(ulong ownerUserIdentifier, string name) =>
            await CreateCampaignAsync(new Campaign(ownerUserIdentifier, name));

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

        public async Task UpdateCampaignAsync(Campaign campaign)
        {
            // If the campaign does not exist in the database, abort
            if (await _context.Campaigns.CountAsync(c => c.Id.Equals(campaign.Id)) <= 0)
                return;

            var dto = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == campaign.Id);
            _mapper.Map(campaign, dto);

            _context.Update(dto);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<ulong>> GetModeratorsAsync(int id)
        {
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == id);
            return await _context.Entry(campaign).Collection(x => x.Moderators).Query().Select(y => y.User).Select(z => z.UserIdentifier).ToListAsync();
        }

        public async Task<ICollection<ulong>> GetChannelsAsync(int id)
        {
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == id);
            return await _context.Entry(campaign).Collection(x => x.Channels).Query().Select(y => y.Id).ToListAsync();
        }

        public async Task<ICollection<Character>> GetCharactersAsync(int id)
        {
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == id);

            // TODO: make an extension method to do all this stuff for you
            var characters = await _context.Entry(campaign).Collection(x => x.Characters).Query()
                .Include(x => x.Statistics).ThenInclude(x => x.Statistic)
                .Include(x => x.Statistics).ThenInclude(x => x.StatisticValue)
                .Include(x => x.EffectMappings).ThenInclude(x => x.Effect).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.Statistic)
                .Include(x => x.EffectMappings).ThenInclude(x => x.Effect).ThenInclude(x => x.StatisticEffects).ThenInclude(x => x.StatisticValue)
                .ToListAsync();

            return _mapper.Map<List<CharacterDto>, List<Character>>(characters);
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
            return campaign.RollOptions;
        }

        public async Task<StatisticOptions> GetStatisticOptionsAsync(int id)
        {
            var campaign = await _context.Campaigns.Where(x => x.Id == id).Include(y => y.StatisticOptions).FirstOrDefaultAsync();
            return campaign.StatisticOptions;
        }
    }
}
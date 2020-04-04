using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Statistics;
using Frags.Database.Statistics;
using Microsoft.EntityFrameworkCore;
using Attribute = Frags.Core.Statistics.Attribute;

namespace Frags.Database.DataAccess
{
    public class EfStatisticProvider : IStatisticProvider
    {
        private readonly RpgContext _context;
        private readonly IMapper _mapper;

        public EfStatisticProvider(RpgContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<Attribute> CreateAttributeAsync(string name)
        {
            var dto = new AttributeDto(name);

            await _context.AddAsync(dto);
            await _context.SaveChangesAsync();

            return _mapper.Map<Attribute>(dto);
        }

        public async Task<Skill> CreateSkillAsync(string name, string attribName)
        {
            var attribDto = await _context.Attributes.FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(attribName));
            if (attribDto == null) return null;

            var dto = new SkillDto(attribDto, name);

            await _context.AddAsync(dto);
            await _context.SaveChangesAsync();
            
            return _mapper.Map<Skill>(dto);
        }

        public async Task DeleteStatisticAsync(Statistic statistic)
        {
            var match = await _context.Statistics.FirstOrDefaultAsync(x => x.Id == statistic.Id);

            if (match != null)
            {
                _context.Remove(match);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Statistic>> GetAllStatisticsAsync()
        {
            return _mapper.Map<List<Statistic>>(await _context.Statistics.ToListAsync());
        }

        public async Task<Statistic> GetStatisticAsync(string name)
        {
            return _mapper.Map<Statistic>(await _context.Statistics.FirstOrDefaultAsync(x => x.AliasesArray.Contains(name, StringComparer.OrdinalIgnoreCase)));
        }

        public async Task<Statistic> GetStatisticFromCampaignAsync(string name, int campaignId)
        {
            return _mapper.Map<Statistic>(await _context.Statistics.FirstOrDefaultAsync(x => x.AliasesArray.Contains(name, StringComparer.OrdinalIgnoreCase) && x.Campaign.Id == campaignId));
        }

        public async Task UpdateStatisticAsync(Statistic statistic)
        {
            var dto = await _context.Statistics.FirstOrDefaultAsync(x => x.Id == statistic.Id);

            // Update DTO instance with updated info from POCO instance
            _mapper.Map(statistic, dto);
            _context.Update(dto);
            
            await _context.SaveChangesAsync();
        }
    }
}
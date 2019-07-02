using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Statistics;
using Microsoft.EntityFrameworkCore;
using Attribute = Frags.Core.Statistics.Attribute;

namespace Frags.Database.DataAccess
{
    public class EfStatisticProvider : IStatisticProvider
    {
        private readonly RpgContext _context;

        public EfStatisticProvider(RpgContext context)
        {
            _context = context;
        }
        
        public async Task<Attribute> CreateAttributeAsync(string name)
        {
            var attribute = new Attribute(name);

            await _context.AddAsync(attribute);
            await _context.SaveChangesAsync();

            return attribute;
        }

        public async Task<Skill> CreateSkillAsync(string name, string attribName)
        {
            var attrib = await _context.Attributes.FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(attribName));
            if (attrib == null) return null;

            var skill = new Skill(attrib, name);

            await _context.AddAsync(skill);
            await _context.SaveChangesAsync();
            
            return skill;
        }

        public async Task DeleteStatisticAsync(Statistic statistic)
        {
            _context.Remove(statistic);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Statistic>> GetAllStatisticsAsync()
        {
            return await _context.Statistics.ToListAsync();
        }

        public async Task<Statistic> GetStatisticAsync(string name)
        {
            return await _context.Statistics.FirstOrDefaultAsync(x => x.AliasesArray.Contains(name, StringComparer.OrdinalIgnoreCase));
        }

        public async Task<Statistic> GetStatisticFromCampaignAsync(string name, int campaignId)
        {
            return await _context.Statistics.FirstOrDefaultAsync(x => x.AliasesArray.Contains(name, StringComparer.OrdinalIgnoreCase) && x.Campaign.Id == campaignId);
        }

        public async Task UpdateStatisticAsync(Statistic statistic)
        {
            _context.Update(statistic);
            await _context.SaveChangesAsync();
        }
    }
}
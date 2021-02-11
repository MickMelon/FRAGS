using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Frags.Core.Campaigns;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Statistics;
using Frags.Database.Models;
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
        
        public async Task<Attribute> CreateAttributeAsync(string name, Campaign campaign = null)
        {
            Attribute attrib = new Attribute(name);

            attrib.Campaign = campaign;

            await _context.AddAsync(attrib);
            await _context.SaveChangesAsync();

            return attrib;
        }

        public async Task<Skill> CreateSkillAsync(string name, string attribName, Campaign campaign = null)
        {
            Attribute attrib = await _context.Attributes.FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(attribName));
            if (attrib == null) return null;

            Skill skill = new Skill(attrib, name);

            skill.Campaign = campaign;

            await _context.AddAsync(skill);
            await _context.SaveChangesAsync();
            
            return skill;
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
            return await _context.Statistics.ToListAsync();
        }

        public async Task<IEnumerable<Statistic>> GetAllStatisticsFromCampaignAsync(Campaign campaign)
        {
            return await _context.Statistics.Where(x => x.Campaign.Id == campaign.Id).ToListAsync();
        }

        public async Task<Statistic> GetStatisticAsync(string name, Campaign campaign = null)
        {
            if (campaign == null)
                return await _context.Statistics/*.AsNoTracking()*/.FirstOrDefaultAsync(x => x.Aliases.Split(Statistic.ALIAS_SEPARATOR, StringSplitOptions.None).Contains(name, StringComparer.OrdinalIgnoreCase));

            return await _context.Statistics/*.AsNoTracking()*/.FirstOrDefaultAsync(x => x.Aliases.Split(Statistic.ALIAS_SEPARATOR, StringSplitOptions.None).Contains(name, StringComparer.OrdinalIgnoreCase) && x.Campaign.Id == campaign.Id);
        }

        public async Task<Statistic> GetStatisticAsync(int id)
        {
            return await _context.Statistics.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateStatisticAsync(Statistic statistic)
        {
            _context.Update(statistic);
            await _context.SaveChangesAsync();
        }
    }
}
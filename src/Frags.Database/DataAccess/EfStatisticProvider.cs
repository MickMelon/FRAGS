using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Statistics;
using Microsoft.EntityFrameworkCore;

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
            return attribute;
        }

        public async Task<Skill> CreateSkillAsync(string name, string attribName)
        {
            var attrib = await _context.Attributes.Where(x => x.Name.EqualsIgnoreCase(name)).FirstOrDefaultAsync();

            var skill = new Skill(attrib, name);
            await _context.AddAsync(skill);
            return skill;
        }

        public async Task<Statistic> GetStatisticAsync(string name)
        {
            return await _context.Query<Statistic>().Where(x => x.Name.EqualsIgnoreCase(name)).FirstOrDefaultAsync();
        }

        public async Task UpdateStatisticAsync(Statistic statistic)
        {
            _context.Update(statistic);
            await _context.SaveChangesAsync();
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Statistics;
using Frags.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Frags.Database.DataAccess
{
    public class EfStatisticProvider : IStatisticProvider
    {
        private readonly IRepository<Statistic> _statRepo;

        public EfStatisticProvider(IRepository<Statistic> statRepo)
        {
            _statRepo = statRepo;
        }
        
        public async Task<Attribute> CreateAttributeAsync(string name)
        {
            var attribute = new Attribute(name);
            await _statRepo.AddAsync(attribute);
            return attribute;
        }

        public async Task<Skill> CreateSkillAsync(string name, string attribName)
        {
            var attrib = await _statRepo.Query.OfType<Attribute>().Where(x => x.Name.EqualsIgnoreCase(name)).FirstOrDefaultAsync();

            var skill = new Skill(attrib, name);
            await _statRepo.AddAsync(skill);
            return skill;
        }

        public async Task<Statistic> GetStatisticAsync(string name)
        {
            return await _statRepo.Query.Where(x => x.Name.EqualsIgnoreCase(name)).FirstOrDefaultAsync();
        }

        public async Task UpdateStatisticAsync(Statistic statistic)
        {
            await _statRepo.SaveAsync(statistic);
        }
    }
}
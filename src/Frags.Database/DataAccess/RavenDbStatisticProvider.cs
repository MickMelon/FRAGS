using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Frags.Core.Characters;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Statistics;
using Frags.Database.Characters;
using Frags.Database.Repositories;
using Raven.Client.Documents;

namespace Frags.Database.DataAccess
{
    public class RavenDbStatisticProvider : IStatisticProvider
    {
        private readonly IDocumentStore _store;

        public RavenDbStatisticProvider(IDocumentStore store)
        {
            _store = store;
        }

        public async Task<Attribute> CreateAttributeAsync(string name)
        {
            var attrib = new Attribute(name);
            using (var asyncSession = _store.OpenAsyncSession())
            {
                await asyncSession.StoreAsync(attrib);
                await asyncSession.SaveChangesAsync();
            }
            return attrib;
        }

        public async Task<Skill> CreateSkillAsync(string name, string attribName)
        {
            Skill skill;
            using (var asyncSession = _store.OpenAsyncSession())
            {
                var attrib = await asyncSession.Query<Attribute>().FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(attribName));
                skill = new Skill(attrib, name);
                await asyncSession.StoreAsync(skill);
                await asyncSession.SaveChangesAsync();
            }
            return skill;
        }

        public async Task<Statistic> GetStatisticAsync(string name)
        {
            using (var asyncSession = _store.OpenAsyncSession())
            {
                return await asyncSession.Query<Statistic>().FirstOrDefaultAsync(x => x.Name.EqualsIgnoreCase(name));
            }
        }

        public async Task UpdateStatisticAsync(Statistic statistic)
        {
            using (var asyncSession = _store.OpenAsyncSession())
            {
                var stat = await asyncSession.LoadAsync<Statistic>(statistic.Id);
                stat = statistic;
                await asyncSession.SaveChangesAsync();
            }
        }
    }
}
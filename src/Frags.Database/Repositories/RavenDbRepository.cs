using Frags.Database;
using Microsoft.EntityFrameworkCore;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Frags.Database.Repositories
{
    public class RavenDbRepository<T> : IRepository<T> where T : BaseModel
    {
        private readonly IAsyncDocumentSession _asyncSession;
        private readonly IDocumentSession _session;

        public RavenDbRepository(IAsyncDocumentSession asyncSession, IDocumentSession session)
        {
            _asyncSession = asyncSession;
            _session = session;
        }

        /// <summary>
        /// Gets an IQueryable object from the database.
        /// of set T
        /// </summary>
        public IQueryable<T> Query => _asyncSession.Query<T>();

        /// <summary>
        /// Adds an entity of type T to the database.
        /// </summary>
        public void Add(T entity)
        { 
            _session.Store(entity);
            _session.SaveChanges();
        }

        /// <summary>
        /// Adds an entity of type T to the database 
        /// asynchronously.
        /// </summary>
        public async Task AddAsync(T entity) 
        {
            await _asyncSession.StoreAsync(entity);
            await _asyncSession.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an entity of type T from the database.
        /// </summary>
        public void Delete(T entity)
        {
            _session.Delete(entity);
            _session.SaveChanges();
        }

        /// <summary>
        /// Deletes an entity of type T from the database
        /// asynchronously.
        /// </summary>
        public async Task DeleteAsync(T entity) 
        {
            _asyncSession.Delete(entity);
            await _asyncSession.SaveChangesAsync();
        }

        /// <summary>
        /// Fetch all the records of type T from the database.
        /// </summary>
        /// <remarks>
        /// Could be costly with a large amount of records.
        /// </remarks>
        public List<T> FetchAll() => Query.ToList();

        /// <summary>
        /// Fetch all the records of type T from the database
        /// asynchronously.
        /// </summary>
        /// <remarks>
        /// Could be costly with a large amount of records.
        /// </remarks>
        public async Task<List<T>> FetchAllAsync() => await Query.ToListAsync();

        /// <summary>
        /// Saves an entity of type T to the database.
        /// </summary>
        public void Save(T entity) => Add(entity);

        /// <summary>
        /// Saves an entity of type T to the database
        /// asynchronously.
        /// </summary>
        public async Task SaveAsync(T entity) => await AddAsync(entity);
    }
}

using Frags.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Frags.Database.Repositories
{
    public class EfThreadSafeRepository<T> : IRepository<T> where T : BaseModel
    {
        private readonly RpgContext _context;

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public EfThreadSafeRepository(RpgContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets an IQueryable object from the database.
        /// of set T
        /// </summary>
        public IQueryable<T> Query
        {
            get
            {
                _semaphore.Wait();

                try
                {
                    return _context.Set<T>().AsQueryable();
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        /// <summary>
        /// Adds an entity of type T to the database.
        /// </summary>
        public void Add(T entity)
        {
            _semaphore.Wait();

            try
            {
                _context.Add(entity);
                _context.SaveChanges();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Adds an entity of type T to the database 
        /// asynchronously.
        /// </summary>
        public async Task AddAsync(T entity)
        {
            await _semaphore.WaitAsync();

            try
            {
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Deletes an entity of type T from the database.
        /// </summary>
        public void Delete(T entity)
        {
            _semaphore.Wait();

            try
            {
                _context.Remove(entity);
                _context.SaveChanges();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Deletes an entity of type T from the database
        /// asynchronously.
        /// </summary>
        public async Task DeleteAsync(T entity)
        {
            await _semaphore.WaitAsync();

            try
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Fetch all the records of type T from the database.
        /// </summary>
        /// <remarks>
        /// Could be costly with a large amount of records.
        /// </remarks>
        public List<T> FetchAll()
        {
            _semaphore.Wait();

            try
            {
                return _context.Set<T>().ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Fetch all the records of type T from the database
        /// asynchronously.
        /// </summary>
        /// <remarks>
        /// Could be costly with a large amount of records.
        /// </remarks>
        public async Task<List<T>> FetchAllAsync()
        {
            await _semaphore.WaitAsync();

            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Saves an entity of type T to the database.
        /// </summary>
        public void Save(T entity)
        {
            _semaphore.Wait();

            try
            {
                _context.Update(entity);
                _context.SaveChanges();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Saves an entity of type T to the database
        /// asynchronously.
        /// </summary>
        public async Task SaveAsync(T entity)
        {
            await _semaphore.WaitAsync();

            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}

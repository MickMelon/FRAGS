using Frags.Core.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frags.Database.Repositories
{
    public interface IRepository<T> where T : BaseModel
    {
        List<T> FetchAll();
        IQueryable<T> Query { get; }
        void Add(T entity);
        void Delete(T entity);
        void Save(T entity);

        Task<List<T>> FetchAllAsync();
        Task AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveAsync(T entity);
    }
}

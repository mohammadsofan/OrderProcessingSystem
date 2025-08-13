using System.Linq.Expressions;

namespace OrderProcessingSystem.Api.Interfaces.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllByFilterAsync(Expression<Func<T,bool>>? filter=null,
            params Expression<Func<T,object>>[] includes);
        Task<T?> GetOneByFilterAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task AddAsync(T entity);
        Task UpdateAsync(int id,T entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}

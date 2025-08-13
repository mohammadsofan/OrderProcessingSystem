using System.Linq.Expressions;

namespace OrderProcessingSystem.Api.Interfaces.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllByFilterAsync(Expression<Func<T,bool>>? filter=null);
        Task<T?> GetOneByFilterAsync(Expression<Func<T, bool>> filter);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(int id,T entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}

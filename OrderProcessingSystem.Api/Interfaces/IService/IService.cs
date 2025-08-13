using OrderProcessingSystem.Api.Wrappers;
using System.Linq.Expressions;

namespace OrderProcessingSystem.Api.Interfaces.IService
{
    public interface IService<TRequest,TResponse,TEntity> 
        where TEntity : class
        where TRequest : class
        where TResponse : class
    {
        Task<ServiceResult<IEnumerable<TResponse>>> GetAllByFilterAsync(Expression<Func<TEntity, bool>>? filter = null);
        Task<ServiceResult<TResponse?>> GetByIdAsync(int id);
        Task<ServiceResult<TResponse?>> GetOneByFilterAsync(Expression<Func<TEntity, bool>> filter);
        Task<ServiceResult<TResponse>> AddAsync(TRequest entity);
        Task<ServiceResult> UpdateAsync(int id, TRequest entity);
        Task<ServiceResult> DeleteAsync(int id);
    }
}

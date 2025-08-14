using Mapster;
using OrderProcessingSystem.Api.Enums;
using OrderProcessingSystem.Api.Exceptions;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using OrderProcessingSystem.Api.Interfaces.IService;
using OrderProcessingSystem.Api.Wrappers;
using System.Linq.Expressions;

namespace OrderProcessingSystem.Api.Services
{
    public class Service<TRequest, TResponse, TEntity> : IService<TRequest, TResponse, TEntity>
        where TEntity : class
        where TRequest : class
        where TResponse : class
    {
        private readonly IRepository<TEntity> _repository;

        public Service(IRepository<TEntity> repository)
        {
            _repository = repository;
        }
        public async Task<ServiceResult<TResponse>> AddAsync(TRequest entity)
        {
            try
            {
                var entityToAdd = entity.Adapt<TEntity>();
                await _repository.AddAsync(entityToAdd);
                var response = entityToAdd.Adapt<TResponse>();
                return ServiceResult<TResponse>.Success(response, "Entity added successfully.");
            }
            catch (ConflictDbException ex)
            {
                return ServiceResult<TResponse>.Failure($"Conflict error: {ex.Message}", ServiceResultStatus.Conflict);
            }
            catch (EntityNotFoundException ex)
            {
                return ServiceResult<TResponse>.Failure($"Entity not found: {ex.Message}", ServiceResultStatus.NotFound);
            }
            catch (Exception ex)
            {
                return ServiceResult<TResponse>.Failure($"Error while adding entity: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return ServiceResult.Success("Entity deleted successfully.");
            }
            catch (EntityNotFoundException ex)
            {
                return ServiceResult.Failure($"Entity not found: {ex.Message}", ServiceResultStatus.NotFound);
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error while deleting entity: {ex.Message}");
            }
        }

        public virtual async Task<ServiceResult<IEnumerable<TResponse>>> GetAllByFilterAsync(Expression<Func<TEntity, bool>>? filter = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                var response = (await _repository.GetAllByFilterAsync(filter,includes))
                    .Select(entity => entity.Adapt<TResponse>())
                    .ToList();
                return ServiceResult<IEnumerable<TResponse>>.Success(response,
                    "Entities fetched successfully."
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<TResponse>>.Failure($"Error while fetching entities: {ex.Message}");
            }
        }

        public virtual async Task<ServiceResult<TResponse?>> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id,includes);
                if (entity == null)
                {
                    return ServiceResult<TResponse?>.Failure("Entity not found.", ServiceResultStatus.NotFound);
                }
                else
                {
                    return ServiceResult<TResponse?>.Success(entity.Adapt<TResponse>(), "Entity fetched successfully.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<TResponse?>.Failure($"Error while fetching entity: {ex.Message}");
            }
        }

        public virtual async Task<ServiceResult<TResponse?>> GetOneByFilterAsync(Expression<Func<TEntity, bool>> filter
            , params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                var entity = await _repository.GetOneByFilterAsync(filter, includes);
                if (entity == null)
                {
                    return ServiceResult<TResponse?>.Failure("Entity not found.", ServiceResultStatus.NotFound);
                }
                else
                {
                    return ServiceResult<TResponse?>.Success(entity.Adapt<TResponse>(), "Entity fetched successfully.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<TResponse?>.Failure($"Error while fetching entity: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateAsync(int id, TRequest entity)
        {
            try
            {
                var entityToUpdate = entity.Adapt<TEntity>();
                await _repository.UpdateAsync(id, entityToUpdate);
                return ServiceResult.Success("Entity updated successfully.");
            }
            catch (ConflictDbException ex)
            {
                return ServiceResult.Failure($"Conflict error: {ex.Message}", ServiceResultStatus.Conflict);
            }
            catch (EntityNotFoundException ex)
            {
                return ServiceResult.Failure($"Entity not found: {ex.Message}", ServiceResultStatus.NotFound);
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Error while updating entity: {ex.Message}");
            }
        }
    }
}

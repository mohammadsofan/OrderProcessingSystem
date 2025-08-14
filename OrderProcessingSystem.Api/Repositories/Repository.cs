using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Api.Data;
using OrderProcessingSystem.Api.Exceptions;
using OrderProcessingSystem.Api.Interfaces;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using System.Linq.Expressions;

namespace OrderProcessingSystem.Api.Repositories
{
    public class Repository<T> : IRepository<T> where T : class,IEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet=context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new ConflictDbException("An error occurred while adding the entity to the database.", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    throw new EntityNotFoundException($"Entity with id {id} not found.");
                }
                _dbSet.Remove(entity);
                await SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetAllByFilterAsync(Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> queryable = _dbSet.AsNoTracking();
                if (filter != null)
                {
                    queryable = queryable.Where(filter);
                }
                foreach (var include in includes)
                {
                    queryable = queryable.Include(include);
                }
                return await queryable.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> queryable = _dbSet.AsNoTracking();
                foreach (var include in includes)
                {
                    queryable = queryable.Include(include);
                }
                queryable = queryable.Where(e=>e.Id == id);
                return await queryable.FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T?> GetOneByFilterAsync(Expression<Func<T, bool>> filter,
            params Expression<Func<T, object>>[] includes)
        {
            try
            {
                IQueryable<T> queryable = _dbSet.AsNoTracking();
                foreach (var include in includes)
                {
                    queryable = queryable.Include(include);
                }
                queryable = queryable.Where(filter);
                return await queryable.FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try { 
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAsync(int id,T entity)
        {
            try
            {
                var existingEntity = await _dbSet.FindAsync(id);
                if (existingEntity == null)
                {
                    throw new EntityNotFoundException($"Entity with id {id} not found.");
                }
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                await SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new ConflictDbException("An error occurred while updating the entity in the database.", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

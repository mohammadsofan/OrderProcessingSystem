using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Api.Data;
using OrderProcessingSystem.Api.Exceptions;
using OrderProcessingSystem.Api.Interfaces.IRepository;
using System.Linq.Expressions;

namespace OrderProcessingSystem.Api.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
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
            catch(Exception)
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

        public async Task<IEnumerable<T>> GetAllByFilterAsync(Expression<Func<T, bool>>? filter = null)
        {
            try
            {
                IQueryable<T> queryable = _dbSet.AsNoTracking();
                if (filter != null)
                {
                    queryable = queryable.Where(filter);
                }
                return await queryable.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T?> GetOneByFilterAsync(Expression<Func<T, bool>> filter)
        {
            try
            {
                return await _dbSet.FirstOrDefaultAsync(filter);
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}

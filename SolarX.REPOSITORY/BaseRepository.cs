using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY;

public class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
{
    private readonly ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddEntity(TEntity entity)
    {
        _context.Add(entity);
    }

    public IQueryable<TEntity> GetAllWithQuery(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return query;
    }

    public async Task<TEntity?> GetById(TKey id, params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        var query = await GetAllWithQuery(x => x.Id!.Equals(id), includeProperties)
            .AsTracking().SingleOrDefaultAsync();
        return query;
    }

    public async Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        var query = await GetAllWithQuery(predicate, includeProperties)
            .AsTracking()
            .SingleOrDefaultAsync();
        return query;
    }

    public void RemoveEntity(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public void UpdateEntity(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    public void AddBulkAsync(IEnumerable<TEntity> entities)
    {
         _context.AddRangeAsync(entities);
    }

    public void UpdateBulk(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().UpdateRange(entities);
    }

    public void RemoveBulk(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().RemoveRange(entities);
    }
}
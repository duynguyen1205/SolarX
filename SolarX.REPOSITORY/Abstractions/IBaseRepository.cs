using System.Linq.Expressions;


namespace SolarX.REPOSITORY.Abstractions;

public interface IBaseRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    void AddEntity(TEntity entity);

    IQueryable<TEntity> GetAllWithQuery(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[]? includeProperties);

    Task<TEntity?> GetById(TKey id, params Expression<Func<TEntity, object>>[]? includeProperties);

    Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[]? includeProperties);

    void RemoveEntity(TEntity entity);
    void UpdateEntity(TEntity entity);
    void AddBulkAsync(IEnumerable<TEntity> entities);
    void UpdateBulk(IEnumerable<TEntity> entities);
    void RemoveBulk(IEnumerable<TEntity> entities);
}
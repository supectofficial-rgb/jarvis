namespace OysterFx.AppCore.Shared.Commands;

using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Linq.Expressions;

public interface ICommandRepository<TEntity, TId> where TEntity : AggregateRoot<TId> where TId : struct, IComparable, IComparable<TId>, IConvertible, IEquatable<TId>, IFormattable
{
    void Delete(TId id);
    void DeleteGraph(TId id);
    void Delete(TEntity entity);
    void Insert(TEntity entity);
    Task InsertAsync(TEntity entity);
    TEntity Get(TId id);
    Task<TEntity> GetAsync(TId id);
    TEntity Get(BusinessKey BusinessKey);
    Task<TEntity> GetAsync(BusinessKey BusinessKey);
    TEntity GetGraph(TId id);
    Task<TEntity> GetGraphAsync(TId id);
    TEntity GetGraph(BusinessKey BusinessKey);
    Task<TEntity> GetGraphAsync(BusinessKey BusinessKey);
    bool Exists(Expression<Func<TEntity, bool>> expression);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression);
    int Commit();
    Task<int> CommitAsync();
}
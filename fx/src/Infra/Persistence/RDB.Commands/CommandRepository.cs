namespace OysterFx.Infra.Persistence.RDB.Commands;

using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.AppCore.Shared.Commands;
using System.Linq.Expressions;

public class CommandRepository<TEntity, TDbContext, TId> : ICommandRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TDbContext : CommandDbContext
     where TId : struct,
          IComparable,
          IComparable<TId>,
          IConvertible,
          IEquatable<TId>,
          IFormattable
{

    protected readonly TDbContext _dbContext;

    public CommandRepository(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Delete(TId id)
    {
        var entity = _dbContext.Set<TEntity>().Find(id);
        _dbContext.Set<TEntity>().Remove(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
    }

    public void DeleteGraph(TId id)
    {
        var entity = GetGraph(id);
        if (entity is not null && !entity.Id.Equals(default))
            _dbContext.Set<TEntity>().Remove(entity);
    }
    #region insert

    public void Insert(TEntity entity)
    {
        _dbContext.Set<TEntity>().Add(entity);
    }

    public async Task InsertAsync(TEntity entity)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity);
    }
    #endregion

    #region Get Single Item
    public TEntity Get(TId id)
    {
        return _dbContext.Set<TEntity>().Find(id);
    }

    public TEntity Get(BusinessKey businessKey)
    {
        return _dbContext.Set<TEntity>().FirstOrDefault(c => c.BusinessKey == businessKey)!;
    }

    public async Task<TEntity> GetAsync(TId id)
    {
        return await _dbContext.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity> GetAsync(BusinessKey BusinessKey)
    {
        return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(c => c.BusinessKey == BusinessKey)!;
    }
    #endregion

    #region Get single item with graph
    public TEntity GetGraph(TId id)
    {
        var graphPath = _dbContext.GetIncludePaths(typeof(TEntity));
        IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsQueryable();
        var temp = graphPath.ToList();
        foreach (var item in graphPath)
        {
            query = query.Include(item);
        }
        return query.FirstOrDefault(c => c.Id.Equals(id));
    }

    public TEntity GetGraph(BusinessKey BusinessKey)
    {
        var graphPath = _dbContext.GetIncludePaths(typeof(TEntity));
        IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsQueryable();
        var temp = graphPath.ToList();
        foreach (var item in graphPath)
        {
            query = query.Include(item);
        }
        return query.FirstOrDefault(c => c.BusinessKey == BusinessKey)!;
    }

    public async Task<TEntity> GetGraphAsync(TId id)
    {
        var graphPath = _dbContext.GetIncludePaths(typeof(TEntity));
        IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath)
        {
            query = query.Include(item);
        }
        return await query.FirstOrDefaultAsync(c => c.Id.Equals(id));
    }

    public async Task<TEntity> GetGraphAsync(BusinessKey BusinessKey)
    {
        var graphPath = _dbContext.GetIncludePaths(typeof(TEntity));
        IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsQueryable();
        foreach (var item in graphPath)
        {
            query = query.Include(item);
        }
        return await query.FirstOrDefaultAsync(c => c.BusinessKey == BusinessKey);
    }
    #endregion

    #region Exists
    public bool Exists(Expression<Func<TEntity, bool>> expression)
    {
        return _dbContext.Set<TEntity>().Any(expression);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _dbContext.Set<TEntity>().AnyAsync(expression);
    }
    #endregion

    #region Transaction management
    public int Commit()
    {
        return _dbContext.SaveChanges();
    }

    public Task<int> CommitAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
    #endregion
}

public class CommandRepository<TEntity, TDbContext> : CommandRepository<TEntity, TDbContext, long>
    where TEntity : AggregateRoot
    where TDbContext : CommandDbContext
{
    public CommandRepository(TDbContext dbContext) : base(dbContext) { }
}
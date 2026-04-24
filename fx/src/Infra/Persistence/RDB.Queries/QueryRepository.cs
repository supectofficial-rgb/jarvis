namespace OysterFx.Infra.Persistence.RDB.Queries;

using OysterFx.AppCore.Shared.Queries;

public class QueryRepository<TDbContext> : IQueryRepository
    where TDbContext : QueryDbContext
{
    protected readonly TDbContext _dbContext;
    public QueryRepository(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}
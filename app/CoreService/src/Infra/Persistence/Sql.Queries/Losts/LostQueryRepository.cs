namespace Insurance.Infra.Persistence.Sql.Queries.Losts;

using Insurance.AppCore.Shared.Losts.Queries;
using OysterFx.Infra.Persistence.RDB.Queries;

public class LostQueryRepository : QueryRepository<InsuranceQueryDbContext>, ILostQueryRepository
{
    public LostQueryRepository(InsuranceQueryDbContext dbContext) : base(dbContext)
    {
    }
}
namespace Insurance.Infra.Persistence.Sql.Commands.Losts;

using Insurance.AppCore.Domain.Parvandes.Entities;
using Insurance.AppCore.Shared.Losts.Commands;
using OysterFx.Infra.Persistence.RDB.Commands;

public class LostCommandRepository : CommandRepository<Lost, InsuranceCommandDbContext>, ILostCommandRepository
{
    public LostCommandRepository(InsuranceCommandDbContext dbContext) : base(dbContext)
    {
    }
}
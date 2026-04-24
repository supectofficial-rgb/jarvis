namespace Insurance.Infra.Persistence.Sql.Queries;

using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class InsuranceQueryDbContext : QueryDbContext
{
    public InsuranceQueryDbContext(DbContextOptions<InsuranceQueryDbContext> options) : base(options)
    {
    }
}
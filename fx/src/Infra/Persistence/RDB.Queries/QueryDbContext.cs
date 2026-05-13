namespace OysterFx.Infra.Persistence.RDB.Queries;

using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Auth.UserServices;
using OysterFx.Infra.Persistence.RDB.Queries.Extensions;

public abstract class QueryDbContext : DbContext
{
    private readonly IUserInfoService? _userInfoService;

    public QueryDbContext(DbContextOptions options, IUserInfoService? userInfoService = null) : base(options)
    {
        _userInfoService = userInfoService;
    }

    public string? CurrentOrganizationBusinessKey => _userInfoService.GetActiveOrganizationBusinessKey();

    protected void AddOrganizationShadowProperties(ModelBuilder builder)
        => builder.AddOrganizationShadowProperties(() => CurrentOrganizationBusinessKey);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    public override int SaveChanges()
    {
        throw new NotSupportedException();
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new NotSupportedException();

    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();

    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();

    }
}

namespace Insurance.HubService.Infra.Persistence.RDB.Queries;

using Insurance.HubService.Infra.Persistence.RDB.Queries.Conversations.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class HubServiceQueryDbContext : QueryDbContext
{
    public HubServiceQueryDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<ConversationSessionReadModel> ConversationSessions { get; set; }
    public DbSet<ConversationMessageReadModel> ConversationMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HubServiceQueryDbContext).Assembly);
    }
}

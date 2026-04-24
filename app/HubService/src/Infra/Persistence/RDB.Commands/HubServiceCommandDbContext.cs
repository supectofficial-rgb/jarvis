namespace Insurance.HubService.Infra.Persistence.RDB.Commands;

using Insurance.HubService.Infra.Persistence.RDB.Commands.Conversations.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class HubServiceCommandDbContext : CommandDbContext
{
    public HubServiceCommandDbContext(DbContextOptions<HubServiceCommandDbContext> options)
        : base(options)
    {
    }

    public DbSet<ConversationSessionEntity> ConversationSessions { get; set; }
    public DbSet<ConversationMessageEntity> ConversationMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HubServiceCommandDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}

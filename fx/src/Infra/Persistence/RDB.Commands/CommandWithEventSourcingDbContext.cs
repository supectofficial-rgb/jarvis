namespace OysterFx.Infra.Persistence.RDB.Commands;

using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.EventSourcing.Abstractions;

public abstract class CommandWithEventSourcingDbContext : CommandDbContext
{
    public DbSet<OutboxEvent> Events { get; set; }

    protected CommandWithEventSourcingDbContext() { }
    protected CommandWithEventSourcingDbContext(DbContextOptions options) : base(options) { }
}

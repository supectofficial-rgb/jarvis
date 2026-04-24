namespace OysterFx.Infra.EventBus.Inbox;

using Microsoft.EntityFrameworkCore;

public sealed class EfCoreInboxStore<TDbContext> : IInboxStore where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public EfCoreInboxStore(TDbContext dbContext) => _dbContext = dbContext;

    public async Task<bool> IsProcessedAsync(Guid eventId, CancellationToken ct)
        => await _dbContext.Set<InboxMessage>().AnyAsync(x => x.EventId == eventId, ct);

    public async Task MarkProcessedAsync(Guid eventId, CancellationToken ct)
    {
        _dbContext.Set<InboxMessage>().Add(new InboxMessage
        {
            EventId = eventId,
            ProcessedAtUtc = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync(ct);
    }
}
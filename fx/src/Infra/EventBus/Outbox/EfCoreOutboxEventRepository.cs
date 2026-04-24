namespace OysterFx.Infra.EventBus.Outbox;

using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.EventSourcing.Abstractions;

public sealed class EfCoreOutboxEventRepository<TDbContext> : IOutboxEventRepository where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public EfCoreOutboxEventRepository(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<OutboxEvent> ReadEvents(int size = 10)
    {
        var take = size <= 0 ? 10 : size;

        return _dbContext.Set<OutboxEvent>()
            .AsNoTracking()
            .Where(x => !x.IsProcessed)
            .OrderBy(x => x.AccuredOn)
            .Take(take)
            .ToList();
    }

    public bool MarkAsRead(IEnumerable<OutboxEvent> events)
    {
        var ids = events
            .Select(x => x.OutBoxEventItemId)
            .Distinct()
            .ToArray();

        if (ids.Length == 0)
            return true;

        var trackedItems = _dbContext.Set<OutboxEvent>()
            .Where(x => ids.Contains(x.OutBoxEventItemId))
            .ToList();

        foreach (var item in trackedItems)
            item.IsProcessed = true;

        _dbContext.SaveChanges();
        return true;
    }
}

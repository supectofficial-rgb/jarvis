using OysterFx.AppCore.Shared.DependencyInjections;
using System.Collections.Concurrent;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Audit;

public sealed class InMemoryAuditTrailService : IAuditTrailService, ISingletoneLifetimeMarker
{
    private readonly ConcurrentQueue<TurnAuditRecord> _records = new();

    public Task RecordAsync(TurnAuditRecord record, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _records.Enqueue(record);

        while (_records.Count > 1000)
        {
            _records.TryDequeue(out _);
        }

        return Task.CompletedTask;
    }
}





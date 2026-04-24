namespace OysterFx.Infra.Persistence.RDB.Commands.Interceptors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.Infra.Auth.UserServices;
using OysterFx.Infra.Persistence.EventSourcing.Abstractions;
using System.Diagnostics;

public class AddOutBoxEventSourcingInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AddOutbox(eventData);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AddOutbox(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void AddOutbox(DbContextEventData eventData)
    {
        var context = eventData.Context;
        if (context is null)
            return;

        List<dynamic> changedAggregates = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.State != EntityState.Detached)
            .Select(c => c.Entity as dynamic)
            .Where(c => c.GetEvents() != null && c.GetEvents().Count > 0)
            .ToList();

        if (changedAggregates is null || changedAggregates.Count == 0)
            return;

        var userInfoService = context.GetService<IUserInfoService>();

        var traceId = string.Empty;
        var spanId = string.Empty;

        if (Activity.Current is not null)
        {
            traceId = Activity.Current.TraceId.ToHexString();
            spanId = Activity.Current.SpanId.ToHexString();
        }

        foreach (var aggregate in changedAggregates)
        {
            var events = aggregate.GetEvents();
            foreach (object @event in events)
            {
                context.Add(new OutboxEvent
                {
                    EventId = Guid.NewGuid(),
                    AccuredByUserId = userInfoService.UserIdOrDefault(),
                    AccuredOn = DateTime.Now,
                    AggregateId = aggregate.BusinessKey.ToString(),
                    AggregateName = aggregate.GetType().Name,
                    AggregateTypeName = aggregate.GetType().FullName ?? aggregate.GetType().Name,
                    EventName = @event.GetType().Name,
                    EventTypeName = @event.GetType().FullName ?? @event.GetType().Name,
                    EventPayload = JsonConvert.SerializeObject(@event),
                    TraceId = traceId,
                    SpanId = spanId,
                    IsProcessed = false
                });
            }
        }
    }
}

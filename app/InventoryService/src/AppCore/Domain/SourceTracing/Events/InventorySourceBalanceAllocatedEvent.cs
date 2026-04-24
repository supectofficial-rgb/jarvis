namespace Insurance.InventoryService.AppCore.Domain.SourceTracing.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record InventorySourceBalanceAllocatedEvent(BusinessKey InventorySourceBalanceBusinessKey, DateTime OccurredOn) : IDomainEvent;

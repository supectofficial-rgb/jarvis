namespace Insurance.InventoryService.AppCore.Domain.SourceTracing.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record InventorySourceAllocationCreatedEvent(BusinessKey InventorySourceAllocationBusinessKey, DateTime OccurredOn) : IDomainEvent;

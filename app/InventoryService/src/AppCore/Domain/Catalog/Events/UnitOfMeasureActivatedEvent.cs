namespace Insurance.InventoryService.AppCore.Domain.Catalog.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record UnitOfMeasureActivatedEvent(BusinessKey UnitOfMeasureBusinessKey, DateTime OccurredOn) : IDomainEvent;

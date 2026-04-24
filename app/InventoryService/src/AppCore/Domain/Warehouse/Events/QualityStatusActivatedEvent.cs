namespace Insurance.InventoryService.AppCore.Domain.Warehouse.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record QualityStatusActivatedEvent(BusinessKey QualityStatusBusinessKey, DateTime OccurredOn) : IDomainEvent;

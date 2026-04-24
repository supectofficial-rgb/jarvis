namespace Insurance.InventoryService.AppCore.Domain.Warehouse.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record LocationMovedToWarehouseEvent(BusinessKey LocationBusinessKey, Guid FromWarehouseRef, Guid ToWarehouseRef, DateTime OccurredOn) : IDomainEvent;

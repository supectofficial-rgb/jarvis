namespace Insurance.InventoryService.AppCore.Domain.StockDetails.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record StockDetailAdjustedEvent(BusinessKey StockDetailBusinessKey, decimal DeltaQuantity, DateTime OccurredOn) : IDomainEvent;

namespace Insurance.InventoryService.AppCore.Domain.StockDetails.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record StockDetailReconciledEvent(BusinessKey StockDetailBusinessKey, DateTime OccurredOn) : IDomainEvent;

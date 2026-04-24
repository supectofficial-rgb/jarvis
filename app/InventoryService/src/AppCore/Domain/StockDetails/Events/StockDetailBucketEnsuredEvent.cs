namespace Insurance.InventoryService.AppCore.Domain.StockDetails.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record StockDetailBucketEnsuredEvent(BusinessKey StockDetailBusinessKey, DateTime OccurredOn) : IDomainEvent;

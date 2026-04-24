namespace Insurance.InventoryService.AppCore.Domain.Fulfillments.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record FulfillmentShippedEvent(BusinessKey FulfillmentBusinessKey, DateTime OccurredOn) : IDomainEvent;

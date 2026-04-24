namespace Insurance.InventoryService.AppCore.Domain.Fulfillments.Events;

using OysterFx.AppCore.Domain.Events;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed record FulfillmentCreatedEvent(BusinessKey FulfillmentBusinessKey, DateTime OccurredOn) : IDomainEvent;

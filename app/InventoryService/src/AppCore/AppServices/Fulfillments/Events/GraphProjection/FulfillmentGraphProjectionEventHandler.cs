namespace Insurance.InventoryService.AppCore.AppServices.Fulfillments.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.Fulfillments.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class FulfillmentGraphProjectionEventHandler :
    IDomainEventHandler<FulfillmentCreatedEvent>,
    IDomainEventHandler<FulfillmentPickedEvent>,
    IDomainEventHandler<FulfillmentPackedEvent>,
    IDomainEventHandler<FulfillmentShippedEvent>,
    IDomainEventHandler<FulfillmentReturnedEvent>
{
    public Task Handle(FulfillmentCreatedEvent @event) => Task.CompletedTask;

    public Task Handle(FulfillmentPickedEvent @event) => Task.CompletedTask;

    public Task Handle(FulfillmentPackedEvent @event) => Task.CompletedTask;

    public Task Handle(FulfillmentShippedEvent @event) => Task.CompletedTask;

    public Task Handle(FulfillmentReturnedEvent @event) => Task.CompletedTask;
}

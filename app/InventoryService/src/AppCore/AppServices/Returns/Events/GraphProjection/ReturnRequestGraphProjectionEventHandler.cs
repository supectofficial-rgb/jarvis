namespace Insurance.InventoryService.AppCore.AppServices.Returns.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.Returns.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class ReturnRequestGraphProjectionEventHandler :
    IDomainEventHandler<ReturnRequestCreatedEvent>,
    IDomainEventHandler<ReturnRequestApprovedEvent>,
    IDomainEventHandler<ReturnRequestRejectedEvent>,
    IDomainEventHandler<ReturnRequestReceivedEvent>,
    IDomainEventHandler<ReturnRequestClosedEvent>
{
    public Task Handle(ReturnRequestCreatedEvent @event) => Task.CompletedTask;

    public Task Handle(ReturnRequestApprovedEvent @event) => Task.CompletedTask;

    public Task Handle(ReturnRequestRejectedEvent @event) => Task.CompletedTask;

    public Task Handle(ReturnRequestReceivedEvent @event) => Task.CompletedTask;

    public Task Handle(ReturnRequestClosedEvent @event) => Task.CompletedTask;
}

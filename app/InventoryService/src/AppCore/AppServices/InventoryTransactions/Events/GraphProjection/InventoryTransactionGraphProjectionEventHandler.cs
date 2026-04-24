namespace Insurance.InventoryService.AppCore.AppServices.InventoryTransactions.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class InventoryTransactionGraphProjectionEventHandler :
    IDomainEventHandler<InventoryTransactionPostedEvent>,
    IDomainEventHandler<InventoryTransactionReversedEvent>,
    IDomainEventHandler<InventoryTransactionCancelledEvent>
{
    public Task Handle(InventoryTransactionPostedEvent @event) => Task.CompletedTask;

    public Task Handle(InventoryTransactionReversedEvent @event) => Task.CompletedTask;

    public Task Handle(InventoryTransactionCancelledEvent @event) => Task.CompletedTask;
}

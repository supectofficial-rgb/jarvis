namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class InventoryDocumentGraphProjectionEventHandler :
    IDomainEventHandler<InventoryDocumentCreatedEvent>,
    IDomainEventHandler<InventoryDocumentPostedEvent>,
    IDomainEventHandler<InventoryDocumentApprovedEvent>,
    IDomainEventHandler<InventoryDocumentRejectedEvent>,
    IDomainEventHandler<InventoryDocumentCancelledEvent>
{
    public Task Handle(InventoryDocumentCreatedEvent @event) => Task.CompletedTask;

    public Task Handle(InventoryDocumentPostedEvent @event) => Task.CompletedTask;

    public Task Handle(InventoryDocumentApprovedEvent @event) => Task.CompletedTask;

    public Task Handle(InventoryDocumentRejectedEvent @event) => Task.CompletedTask;

    public Task Handle(InventoryDocumentCancelledEvent @event) => Task.CompletedTask;
}

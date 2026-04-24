namespace Insurance.InventoryService.AppCore.AppServices.Seller.Sellers.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.Seller.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class SellerGraphProjectionEventHandler :
    IDomainEventHandler<SellerCreatedEvent>,
    IDomainEventHandler<SellerUpdatedEvent>,
    IDomainEventHandler<SellerActivatedEvent>,
    IDomainEventHandler<SellerDeactivatedEvent>,
    IDomainEventHandler<SellerSystemOwnerSetEvent>,
    IDomainEventHandler<SellerSystemOwnerUnsetEvent>
{
    public Task Handle(SellerCreatedEvent @event) => Task.CompletedTask;

    public Task Handle(SellerUpdatedEvent @event) => Task.CompletedTask;

    public Task Handle(SellerActivatedEvent @event) => Task.CompletedTask;

    public Task Handle(SellerDeactivatedEvent @event) => Task.CompletedTask;

    public Task Handle(SellerSystemOwnerSetEvent @event) => Task.CompletedTask;

    public Task Handle(SellerSystemOwnerUnsetEvent @event) => Task.CompletedTask;
}

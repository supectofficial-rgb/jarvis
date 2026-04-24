namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Events.GraphProjection;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Events;
using OysterFx.AppCore.Shared.Events;

public sealed class StockDetailGraphProjectionEventHandler :
    IDomainEventHandler<StockDetailBucketEnsuredEvent>,
    IDomainEventHandler<StockDetailRebuiltEvent>,
    IDomainEventHandler<StockDetailReconciledEvent>,
    IDomainEventHandler<StockDetailArchivedEvent>,
    IDomainEventHandler<StockDetailAdjustedEvent>
{
    public Task Handle(StockDetailBucketEnsuredEvent @event) => Task.CompletedTask;

    public Task Handle(StockDetailRebuiltEvent @event) => Task.CompletedTask;

    public Task Handle(StockDetailReconciledEvent @event) => Task.CompletedTask;

    public Task Handle(StockDetailArchivedEvent @event) => Task.CompletedTask;

    public Task Handle(StockDetailAdjustedEvent @event) => Task.CompletedTask;
}

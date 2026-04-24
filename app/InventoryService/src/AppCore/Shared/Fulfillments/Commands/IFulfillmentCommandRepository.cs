namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands;

using Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IFulfillmentCommandRepository : ICommandRepository<Fulfillment, long>
{
    Task<Fulfillment?> GetByBusinessKeyAsync(Guid fulfillmentBusinessKey);
}

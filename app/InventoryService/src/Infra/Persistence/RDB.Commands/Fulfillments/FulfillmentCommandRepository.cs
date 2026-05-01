namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Fulfillments;

using Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class FulfillmentCommandRepository
    : CommandRepository<Fulfillment, InventoryServiceCommandDbContext>, IFulfillmentCommandRepository
{
    public FulfillmentCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Fulfillment?> GetByBusinessKeyAsync(Guid fulfillmentBusinessKey)
    {
        return _dbContext.Fulfillments
            .Include(x => x.Lines)
            .ThenInclude(x => x.Serials)
            .Include(x => x.Transitions)
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(fulfillmentBusinessKey));
    }
}

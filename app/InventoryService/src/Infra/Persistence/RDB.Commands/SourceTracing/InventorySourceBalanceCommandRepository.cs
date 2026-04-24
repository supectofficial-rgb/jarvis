namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.SourceTracing;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class InventorySourceBalanceCommandRepository
    : CommandRepository<InventorySourceBalance, InventoryServiceCommandDbContext>, IInventorySourceBalanceCommandRepository
{
    public InventorySourceBalanceCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<InventorySourceBalance?> GetByBusinessKeyAsync(Guid sourceBalanceBusinessKey)
    {
        return _dbContext.InventorySourceBalances
            .Include(x => x.Allocations)
            .Include(x => x.Consumptions)
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == sourceBalanceBusinessKey);
    }
}

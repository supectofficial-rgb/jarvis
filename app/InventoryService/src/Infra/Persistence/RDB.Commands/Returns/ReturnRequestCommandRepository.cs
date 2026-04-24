namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Returns;

using Insurance.InventoryService.AppCore.Domain.Returns.Entities;
using Insurance.InventoryService.AppCore.Shared.Returns.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class ReturnRequestCommandRepository
    : CommandRepository<ReturnRequest, InventoryServiceCommandDbContext>, IReturnRequestCommandRepository
{
    public ReturnRequestCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<ReturnRequest?> GetByBusinessKeyAsync(Guid returnRequestBusinessKey)
    {
        return _dbContext.ReturnRequests
            .Include(x => x.Lines)
            .ThenInclude(x => x.Serials)
            .Include(x => x.Transitions)
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == returnRequestBusinessKey);
    }
}

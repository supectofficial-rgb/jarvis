namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.UnitOfMeasures;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class UnitOfMeasureCommandRepository : CommandRepository<UnitOfMeasure, InventoryServiceCommandDbContext>, IUnitOfMeasureCommandRepository
{
    public UnitOfMeasureCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<UnitOfMeasure?> GetByBusinessKeyAsync(Guid unitOfMeasureBusinessKey)
    {
        return _dbContext.Set<UnitOfMeasure>()
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == unitOfMeasureBusinessKey);
    }

    public Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null)
    {
        var normalized = code.Trim();
        var query = _dbContext.Set<UnitOfMeasure>().Where(x => x.Code == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey.Value != exceptBusinessKey.Value);

        return query.AnyAsync();
    }
}

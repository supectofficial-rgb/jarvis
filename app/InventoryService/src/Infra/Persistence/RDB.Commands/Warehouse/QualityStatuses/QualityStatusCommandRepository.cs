namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.QualityStatuses;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class QualityStatusCommandRepository : CommandRepository<QualityStatus, InventoryServiceCommandDbContext>, IQualityStatusCommandRepository
{
    public QualityStatusCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<QualityStatus?> GetByBusinessKeyAsync(Guid qualityStatusBusinessKey)
    {
        return _dbContext.Set<QualityStatus>()
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == qualityStatusBusinessKey);
    }

    public Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null)
    {
        var normalized = code.Trim();
        var query = _dbContext.Set<QualityStatus>().Where(x => x.Code == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey.Value != exceptBusinessKey.Value);

        return query.AnyAsync();
    }
}

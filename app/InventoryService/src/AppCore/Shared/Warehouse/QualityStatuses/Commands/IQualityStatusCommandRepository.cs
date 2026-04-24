namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IQualityStatusCommandRepository : ICommandRepository<QualityStatus, long>
{
    Task<QualityStatus?> GetByBusinessKeyAsync(Guid qualityStatusBusinessKey);

    Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null);
}

namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IUnitOfMeasureCommandRepository : ICommandRepository<UnitOfMeasure, long>
{
    Task<UnitOfMeasure?> GetByBusinessKeyAsync(Guid unitOfMeasureBusinessKey);
    Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null);
}

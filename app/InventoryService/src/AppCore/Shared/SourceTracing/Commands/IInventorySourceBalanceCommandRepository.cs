namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IInventorySourceBalanceCommandRepository : ICommandRepository<InventorySourceBalance, long>
{
    Task<InventorySourceBalance?> GetByBusinessKeyAsync(Guid sourceBalanceBusinessKey);
}

namespace Insurance.InventoryService.AppCore.Shared.Returns.Commands;

using Insurance.InventoryService.AppCore.Domain.Returns.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IReturnRequestCommandRepository : ICommandRepository<ReturnRequest, long>
{
    Task<ReturnRequest?> GetByBusinessKeyAsync(Guid returnRequestBusinessKey);
}

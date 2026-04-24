namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IInventoryDocumentCommandRepository : ICommandRepository<InventoryDocument, long>
{
    Task<InventoryDocument?> GetByBusinessKeyAsync(Guid documentBusinessKey);
    Task<bool> ExistsByDocumentNoAsync(string documentNo, Guid? exceptBusinessKey = null);
}

namespace Insurance.InventoryService.AppCore.Shared.Catalog.Tags.Commands;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface ITagCommandRepository : ICommandRepository<Tag, long>
{
    Task<Tag?> GetByBusinessKeyAsync(Guid tagBusinessKey);
    Task<bool> ExistsByNameAsync(string tagName, Guid? exceptBusinessKey = null);
}

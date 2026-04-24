namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IAttributeDefinitionCommandRepository : ICommandRepository<AttributeDefinition, long>
{
    Task<AttributeDefinition?> GetByBusinessKeyAsync(Guid attributeDefinitionBusinessKey);
    Task<IReadOnlyCollection<AttributeDefinition>> GetByBusinessKeysAsync(IReadOnlyCollection<Guid> attributeDefinitionBusinessKeys);
    Task<bool> ExistsByCodeAsync(string code, Guid? exceptBusinessKey = null);
}

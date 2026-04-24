namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.DeactivateAttributeDefinition;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeactivateAttributeDefinition;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeactivateAttributeDefinitionCommandHandler
    : CommandHandler<DeactivateAttributeDefinitionCommand, DeactivateAttributeDefinitionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;

    public DeactivateAttributeDefinitionCommandHandler(
        IAttributeDefinitionCommandRepository repository,
        ICategoryCommandRepository categoryRepository,
        IProductCommandRepository productRepository,
        IProductVariantCommandRepository variantRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<DeactivateAttributeDefinitionCommandResult>> Handle(DeactivateAttributeDefinitionCommand command)
    {
        if (command.AttributeDefinitionBusinessKey == Guid.Empty)
            return Fail("AttributeDefinitionBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.AttributeDefinitionBusinessKey);
        if (aggregate is null)
            return Fail("Attribute definition was not found.");

        var hasCategoryUsage = await _categoryRepository.ExistsRuleByAttributeRefAsync(command.AttributeDefinitionBusinessKey, onlyActiveCategories: true, onlyActiveRules: true);
        if (hasCategoryUsage)
            return Fail("Attribute definition cannot be deactivated because active category rules depend on it.");

        var hasProductUsage = await _productRepository.ExistsAttributeValueByAttributeRefAsync(command.AttributeDefinitionBusinessKey, onlyActive: true);
        if (hasProductUsage)
            return Fail("Attribute definition cannot be deactivated because active products depend on it.");

        var hasVariantUsage = await _variantRepository.ExistsAttributeValueByAttributeRefAsync(command.AttributeDefinitionBusinessKey, onlyActive: true);
        if (hasVariantUsage)
            return Fail("Attribute definition cannot be deactivated because active variants depend on it.");

        aggregate.Deactivate();
        await _repository.CommitAsync();

        return Ok(new DeactivateAttributeDefinitionCommandResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey.Value,
            IsActive = aggregate.IsActive
        });
    }
}

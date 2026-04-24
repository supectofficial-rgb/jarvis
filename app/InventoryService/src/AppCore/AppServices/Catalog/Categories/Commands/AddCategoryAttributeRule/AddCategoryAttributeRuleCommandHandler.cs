namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Commands.AddCategoryAttributeRule;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.AddCategoryAttributeRule;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class AddCategoryAttributeRuleCommandHandler : CommandHandler<AddCategoryAttributeRuleCommand, AddCategoryAttributeRuleCommandResult>
{
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IAttributeDefinitionCommandRepository _attributeRepository;
    private readonly IProductCommandRepository _productRepository;

    public AddCategoryAttributeRuleCommandHandler(
        ICategoryCommandRepository categoryRepository,
        IAttributeDefinitionCommandRepository attributeRepository,
        IProductCommandRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _attributeRepository = attributeRepository;
        _productRepository = productRepository;
    }

    public override async Task<CommandResult<AddCategoryAttributeRuleCommandResult>> Handle(AddCategoryAttributeRuleCommand command)
    {
        if (command.CategoryBusinessKey == Guid.Empty)
            return Fail("CategoryBusinessKey is required.");

        if (command.AttributeRef == Guid.Empty)
            return Fail("AttributeRef is required.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryBusinessKey);
        if (category is null)
            return Fail("Category was not found.");

        var validationError = await ValidateAttributeAsync(command.AttributeRef, command.IsVariant);
        if (validationError is not null)
            return Fail(validationError);

        var currentSchemaVersionRef = category.CurrentSchemaVersionRef;
        var createNewSchemaVersion = await _productRepository.ExistsByCategorySchemaVersionRefAsync(currentSchemaVersionRef);

        category.AddAttributeRule(
            command.AttributeRef,
            command.IsRequired,
            command.IsVariant,
            command.DisplayOrder,
            command.IsOverridden,
            command.IsActive,
            createNewSchemaVersion,
            changeSummary: $"Add category rule for attribute {command.AttributeRef}");
        await _categoryRepository.CommitAsync();

        return Ok(new AddCategoryAttributeRuleCommandResult
        {
            CategoryBusinessKey = category.BusinessKey.Value,
            CategorySchemaVersionRef = category.CurrentSchemaVersionRef,
            AttributeRef = command.AttributeRef,
            IsActive = command.IsActive
        });
    }

    private async Task<string?> ValidateAttributeAsync(Guid attributeRef, bool isVariant)
    {
        var definition = await _attributeRepository.GetByBusinessKeyAsync(attributeRef);
        if (definition is null)
            return "Attribute definition was not found.";

        if (!definition.IsActive)
            return "Attribute definition is inactive.";

        if (isVariant && definition.Scope == AttributeScope.Product)
            return "Product-scope attribute cannot be used as variant rule.";

        if (!isVariant && definition.Scope == AttributeScope.Variant)
            return "Variant-scope attribute cannot be used as product rule.";

        return null;
    }
}

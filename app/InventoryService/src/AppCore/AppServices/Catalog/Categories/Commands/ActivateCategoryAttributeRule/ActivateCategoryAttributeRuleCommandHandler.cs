namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Commands.ActivateCategoryAttributeRule;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.ActivateCategoryAttributeRule;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ActivateCategoryAttributeRuleCommandHandler : CommandHandler<ActivateCategoryAttributeRuleCommand, ActivateCategoryAttributeRuleCommandResult>
{
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IAttributeDefinitionCommandRepository _attributeRepository;
    private readonly IProductCommandRepository _productRepository;

    public ActivateCategoryAttributeRuleCommandHandler(
        ICategoryCommandRepository categoryRepository,
        IAttributeDefinitionCommandRepository attributeRepository,
        IProductCommandRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _attributeRepository = attributeRepository;
        _productRepository = productRepository;
    }

    public override async Task<CommandResult<ActivateCategoryAttributeRuleCommandResult>> Handle(ActivateCategoryAttributeRuleCommand command)
    {
        if (command.CategoryBusinessKey == Guid.Empty || command.AttributeRef == Guid.Empty)
            return Fail("CategoryBusinessKey and AttributeRef are required.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryBusinessKey);
        if (category is null)
            return Fail("Category was not found.");

        var rule = category.AttributeRules.FirstOrDefault(x => x.AttributeRef == command.AttributeRef);
        if (rule is null)
            return Fail("Category attribute rule was not found.");

        var definition = await _attributeRepository.GetByBusinessKeyAsync(command.AttributeRef);
        if (definition is null || !definition.IsActive)
            return Fail("Attribute definition must be active to activate rule.");

        var currentSchemaVersionRef = category.CurrentSchemaVersionRef;
        var createNewSchemaVersion = await _productRepository.ExistsByCategorySchemaVersionRefAsync(currentSchemaVersionRef);

        category.AddAttributeRule(
            rule.AttributeRef,
            rule.IsRequired,
            rule.IsVariant,
            rule.DisplayOrder,
            rule.IsOverridden,
            true,
            createNewSchemaVersion,
            changeSummary: $"Activate category rule for attribute {rule.AttributeRef}");
        await _categoryRepository.CommitAsync();

        return Ok(new ActivateCategoryAttributeRuleCommandResult
        {
            CategoryBusinessKey = category.BusinessKey.Value,
            CategorySchemaVersionRef = category.CurrentSchemaVersionRef,
            AttributeRef = rule.AttributeRef,
            IsActive = true
        });
    }
}

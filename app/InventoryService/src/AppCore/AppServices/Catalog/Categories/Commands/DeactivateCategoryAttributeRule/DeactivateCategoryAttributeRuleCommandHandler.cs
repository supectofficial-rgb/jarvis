namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Commands.DeactivateCategoryAttributeRule;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.DeactivateCategoryAttributeRule;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeactivateCategoryAttributeRuleCommandHandler : CommandHandler<DeactivateCategoryAttributeRuleCommand, DeactivateCategoryAttributeRuleCommandResult>
{
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IProductCommandRepository _productRepository;

    public DeactivateCategoryAttributeRuleCommandHandler(ICategoryCommandRepository categoryRepository, IProductCommandRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public override async Task<CommandResult<DeactivateCategoryAttributeRuleCommandResult>> Handle(DeactivateCategoryAttributeRuleCommand command)
    {
        if (command.CategoryBusinessKey == Guid.Empty || command.AttributeRef == Guid.Empty)
            return Fail("CategoryBusinessKey and AttributeRef are required.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryBusinessKey);
        if (category is null)
            return Fail("Category was not found.");

        var rule = category.AttributeRules.FirstOrDefault(x => x.AttributeRef == command.AttributeRef);
        if (rule is null)
            return Fail("Category attribute rule was not found.");

        var currentSchemaVersionRef = category.CurrentSchemaVersionRef;
        var createNewSchemaVersion = await _productRepository.ExistsByCategorySchemaVersionRefAsync(currentSchemaVersionRef);

        category.AddAttributeRule(
            rule.AttributeRef,
            rule.IsRequired,
            rule.IsVariant,
            rule.DisplayOrder,
            rule.IsOverridden,
            false,
            createNewSchemaVersion,
            changeSummary: $"Deactivate category rule for attribute {rule.AttributeRef}");
        await _categoryRepository.CommitAsync();

        return Ok(new DeactivateCategoryAttributeRuleCommandResult
        {
            CategoryBusinessKey = category.BusinessKey.Value,
            CategorySchemaVersionRef = category.CurrentSchemaVersionRef,
            AttributeRef = rule.AttributeRef,
            IsActive = false
        });
    }
}

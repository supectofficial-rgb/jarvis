namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Commands.RemoveCategoryAttributeRule;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.RemoveCategoryAttributeRule;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RemoveCategoryAttributeRuleCommandHandler : CommandHandler<RemoveCategoryAttributeRuleCommand, RemoveCategoryAttributeRuleCommandResult>
{
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IProductCommandRepository _productRepository;

    public RemoveCategoryAttributeRuleCommandHandler(ICategoryCommandRepository categoryRepository, IProductCommandRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public override async Task<CommandResult<RemoveCategoryAttributeRuleCommandResult>> Handle(RemoveCategoryAttributeRuleCommand command)
    {
        if (command.CategoryBusinessKey == Guid.Empty || command.AttributeRef == Guid.Empty)
            return Fail("CategoryBusinessKey and AttributeRef are required.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryBusinessKey);
        if (category is null)
            return Fail("Category was not found.");

        var hasProducts = await _productRepository.ExistsByCategoryRefAsync(command.CategoryBusinessKey, onlyActive: false);
        if (hasProducts)
            return Fail("Category attribute rule cannot be removed because products exist for this category.");

        var currentSchemaVersionRef = category.CurrentSchemaVersionRef;
        var createNewSchemaVersion = await _productRepository.ExistsByCategorySchemaVersionRefAsync(currentSchemaVersionRef);

        var existed = category.AttributeRules.Any(x => x.AttributeRef == command.AttributeRef);
        category.RemoveAttributeRule(
            command.AttributeRef,
            createNewSchemaVersion,
            changeSummary: $"Remove category rule for attribute {command.AttributeRef}");
        await _categoryRepository.CommitAsync();

        return Ok(new RemoveCategoryAttributeRuleCommandResult
        {
            CategoryBusinessKey = category.BusinessKey.Value,
            CategorySchemaVersionRef = category.CurrentSchemaVersionRef,
            AttributeRef = command.AttributeRef,
            Removed = existed
        });
    }
}

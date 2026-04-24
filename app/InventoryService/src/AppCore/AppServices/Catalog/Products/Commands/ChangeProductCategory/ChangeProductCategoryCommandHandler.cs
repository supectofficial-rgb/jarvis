namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Commands.ChangeProductCategory;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.ChangeProductCategory;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ChangeProductCategoryCommandHandler : CommandHandler<ChangeProductCategoryCommand, ChangeProductCategoryCommandResult>
{
    private readonly IProductCommandRepository _productRepository;
    private readonly ICategoryCommandRepository _categoryRepository;

    public ChangeProductCategoryCommandHandler(IProductCommandRepository productRepository, ICategoryCommandRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public override async Task<CommandResult<ChangeProductCategoryCommandResult>> Handle(ChangeProductCategoryCommand command)
    {
        if (command.ProductBusinessKey == Guid.Empty)
            return Fail("ProductBusinessKey is required.");

        if (command.CategoryRef == Guid.Empty)
            return Fail("CategoryRef is required.");

        var product = await _productRepository.GetByBusinessKeyAsync(command.ProductBusinessKey);
        if (product is null)
            return Fail("Product was not found.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryRef);
        if (category is null)
            return Fail("Category was not found.");

        var categorySchemaVersionRef = category.CurrentSchemaVersionRef;

        if (product.IsActive && !category.IsActive)
            return Fail("Active product cannot be moved to inactive category.");

        var productRules = category.GetAttributeRules(categorySchemaVersionRef).Where(x => x.IsActive && !x.IsVariant).ToList();
        if (productRules.Count > 0)
        {
            var allowed = productRules.Select(x => x.AttributeRef).ToHashSet();
            var unknown = product.AttributeValues.Where(x => !allowed.Contains(x.AttributeRef)).Select(x => x.AttributeRef).Distinct().ToList();
            if (unknown.Count > 0)
                return Fail("Product has attributes not allowed in target category.");

            var missingRequired = productRules.Where(x => x.IsRequired).Select(x => x.AttributeRef).Where(x => product.AttributeValues.All(a => a.AttributeRef != x)).ToList();
            if (missingRequired.Count > 0)
                return Fail("Target category requires attributes that are missing on product.");
        }

        product.ChangeCategory(command.CategoryRef, categorySchemaVersionRef);
        await _productRepository.CommitAsync();

        return Ok(new ChangeProductCategoryCommandResult
        {
            ProductBusinessKey = product.BusinessKey.Value,
            CategoryRef = product.CategoryRef,
            CategorySchemaVersionRef = product.CategorySchemaVersionRef
        });
    }
}

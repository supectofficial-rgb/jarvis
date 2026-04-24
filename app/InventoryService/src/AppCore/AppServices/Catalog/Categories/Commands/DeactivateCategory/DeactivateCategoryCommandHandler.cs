namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Commands.DeactivateCategory;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.DeactivateCategory;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeactivateCategoryCommandHandler : CommandHandler<DeactivateCategoryCommand, DeactivateCategoryCommandResult>
{
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IProductCommandRepository _productRepository;

    public DeactivateCategoryCommandHandler(ICategoryCommandRepository categoryRepository, IProductCommandRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public override async Task<CommandResult<DeactivateCategoryCommandResult>> Handle(DeactivateCategoryCommand command)
    {
        if (command.CategoryBusinessKey == Guid.Empty)
            return Fail("CategoryBusinessKey is required.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryBusinessKey);
        if (category is null)
            return Fail("Category was not found.");

        var hasActiveProducts = await _productRepository.ExistsByCategoryRefAsync(command.CategoryBusinessKey, onlyActive: true);
        if (hasActiveProducts)
            return Fail("Category cannot be deactivated while active products exist.");

        var hasActiveChildren = await _categoryRepository.ExistsByParentRefAsync(command.CategoryBusinessKey, onlyActive: true);
        if (hasActiveChildren)
            return Fail("Category cannot be deactivated while active child categories exist.");

        category.Deactivate();
        await _categoryRepository.CommitAsync();

        return Ok(new DeactivateCategoryCommandResult
        {
            CategoryBusinessKey = category.BusinessKey.Value,
            IsActive = category.IsActive
        });
    }
}

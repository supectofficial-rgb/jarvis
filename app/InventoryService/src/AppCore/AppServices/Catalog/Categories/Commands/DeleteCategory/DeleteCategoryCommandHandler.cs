namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Commands.DeleteCategory;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.DeleteCategory;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteCategoryCommandHandler : CommandHandler<DeleteCategoryCommand, DeleteCategoryCommandResult>
{
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IProductCommandRepository _productRepository;

    public DeleteCategoryCommandHandler(ICategoryCommandRepository categoryRepository, IProductCommandRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public override async Task<CommandResult<DeleteCategoryCommandResult>> Handle(DeleteCategoryCommand command)
    {
        if (command.CategoryBusinessKey == Guid.Empty)
            return Fail("CategoryBusinessKey is required.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryBusinessKey);
        if (category is null)
            return Fail("Category was not found.");

        var hasChildren = await _categoryRepository.ExistsByParentRefAsync(command.CategoryBusinessKey, onlyActive: false);
        if (hasChildren)
            return Fail("Category cannot be deleted because child categories exist.");

        bool hasProducts;
        try
        {
            hasProducts = await _productRepository.ExistsByCategoryRefAsync(command.CategoryBusinessKey, onlyActive: false);
        }
        catch (Exception ex)
        {
            return Fail($"Checking category products failed: {GetExceptionMessage(ex)}");
        }

        if (hasProducts)
            return Fail("Category cannot be deleted because products exist.");

        try
        {
            await _categoryRepository.DeleteGraphByBusinessKeyAsync(command.CategoryBusinessKey);
            await _categoryRepository.CommitAsync();
        }
        catch (Exception ex)
        {
            return Fail($"Deleting category failed: {ex.GetBaseException().Message}");
        }

        return Ok(new DeleteCategoryCommandResult
        {
            CategoryBusinessKey = category.BusinessKey.Value,
            Deleted = true
        });
    }

    private static string GetExceptionMessage(Exception exception)
    {
        var messages = new List<string>();
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (!string.IsNullOrWhiteSpace(current.Message))
                messages.Add(current.Message);
        }

        return string.Join(" | ", messages.Distinct());
    }
}

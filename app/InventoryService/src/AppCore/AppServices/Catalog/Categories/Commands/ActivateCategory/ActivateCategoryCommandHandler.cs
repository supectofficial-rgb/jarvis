namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Commands.ActivateCategory;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.ActivateCategory;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ActivateCategoryCommandHandler : CommandHandler<ActivateCategoryCommand, ActivateCategoryCommandResult>
{
    private readonly ICategoryCommandRepository _categoryRepository;

    public ActivateCategoryCommandHandler(ICategoryCommandRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public override async Task<CommandResult<ActivateCategoryCommandResult>> Handle(ActivateCategoryCommand command)
    {
        if (command.CategoryBusinessKey == Guid.Empty)
            return Fail("CategoryBusinessKey is required.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryBusinessKey);
        if (category is null)
            return Fail("Category was not found.");

        if (category.ParentCategoryRef.HasValue)
        {
            var parent = await _categoryRepository.GetByBusinessKeyAsync(category.ParentCategoryRef.Value);
            if (parent is null)
                return Fail("Parent category was not found.");

            if (!parent.IsActive)
                return Fail("Category cannot be activated under inactive parent category.");
        }

        category.Activate();
        await _categoryRepository.CommitAsync();

        return Ok(new ActivateCategoryCommandResult
        {
            CategoryBusinessKey = category.BusinessKey.Value,
            IsActive = category.IsActive
        });
    }
}

namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Commands.MoveCategory;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.MoveCategory;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class MoveCategoryCommandHandler : CommandHandler<MoveCategoryCommand, MoveCategoryCommandResult>
{
    private readonly ICategoryCommandRepository _categoryRepository;

    public MoveCategoryCommandHandler(ICategoryCommandRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public override async Task<CommandResult<MoveCategoryCommandResult>> Handle(MoveCategoryCommand command)
    {
        if (command.CategoryBusinessKey == Guid.Empty)
            return Fail("CategoryBusinessKey is required.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryBusinessKey);
        if (category is null)
            return Fail("Category was not found.");

        if (command.ParentCategoryRef.HasValue)
        {
            if (command.ParentCategoryRef.Value == command.CategoryBusinessKey)
                return Fail("Category cannot be parent of itself.");

            var parent = await _categoryRepository.GetByBusinessKeyAsync(command.ParentCategoryRef.Value);
            if (parent is null)
                return Fail("Parent category was not found.");

            if (category.IsActive && !parent.IsActive)
                return Fail("Active category cannot be moved under inactive parent category.");

            if (await CreatesParentCycle(command.CategoryBusinessKey, command.ParentCategoryRef.Value))
                return Fail("Parent category relation creates a cycle.");
        }

        category.ChangeParent(command.ParentCategoryRef);
        await _categoryRepository.CommitAsync();

        return Ok(new MoveCategoryCommandResult
        {
            CategoryBusinessKey = category.BusinessKey.Value,
            ParentCategoryRef = category.ParentCategoryRef
        });
    }

    private async Task<bool> CreatesParentCycle(Guid categoryBusinessKey, Guid candidateParentBusinessKey)
    {
        var current = candidateParentBusinessKey;
        var visited = new HashSet<Guid>();

        while (true)
        {
            if (!visited.Add(current))
                return true;

            if (current == categoryBusinessKey)
                return true;

            var parent = await _categoryRepository.GetByBusinessKeyAsync(current);
            if (parent?.ParentCategoryRef is null)
                return false;

            current = parent.ParentCategoryRef.Value;
        }
    }
}

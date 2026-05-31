namespace Insurance.InventoryService.AppCore.AppServices.Catalog.VariantNameFormulas.Commands.CreateCategoryVariantNameFormula;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands.CreateCategoryVariantNameFormula;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateCategoryVariantNameFormulaCommandHandler
    : CommandHandler<CreateCategoryVariantNameFormulaCommand, CreateCategoryVariantNameFormulaCommandResult>
{
    private readonly ICategoryVariantNameFormulaCommandRepository _repository;
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly ICategoryQueryRepository _categoryQueryRepository;

    public CreateCategoryVariantNameFormulaCommandHandler(
        ICategoryVariantNameFormulaCommandRepository repository,
        ICategoryCommandRepository categoryRepository,
        ICategoryQueryRepository categoryQueryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _categoryQueryRepository = categoryQueryRepository;
    }

    public override async Task<CommandResult<CreateCategoryVariantNameFormulaCommandResult>> Handle(CreateCategoryVariantNameFormulaCommand command)
    {
        if (command.CategoryRef == Guid.Empty)
            return Fail("CategoryRef is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryRef);
        if (category is null)
            return Fail("Category was not found.");

        var normalizedName = command.Name.Trim();
        if (await _repository.ExistsByCategoryAndNameAsync(command.CategoryRef, normalizedName))
            return Fail($"Formula '{normalizedName}' already exists for this category.");

        var allowedAttributes = await _categoryQueryRepository.GetCategoryAttributeRulesByCategoryIdAsync(command.CategoryRef, includeInherited: true, includeInactive: false);
        var allowedAttributeRefs = allowedAttributes.Select(x => x.AttributeRef).ToHashSet();
        var parts = ResolveParts(command);
        var attributeRefs = parts.Select(x => x.AttributeRef).ToList();

        if (attributeRefs.Any(x => !allowedAttributeRefs.Contains(x)))
            return Fail("Formula contains attributes that are not assigned to the selected category.");

        try
        {
            var aggregate = CategoryVariantNameFormula.Create(
                command.CategoryRef,
                normalizedName,
                command.Separator,
                command.IncludeCategoryName,
                command.DisplayOrder,
                parts.Select(x => (x.AttributeRef, x.Separator, x.SortOrder)).ToList(),
                command.IsActive);

            await _repository.InsertAsync(aggregate);
            await _repository.CommitAsync();
            return Ok(new CreateCategoryVariantNameFormulaCommandResult { FormulaBusinessKey = aggregate.BusinessKey.Value });
        }
        catch (Exception ex)
        {
            return Fail($"Creating variant name formula failed: {ex.Message}");
        }
    }

    private static List<CategoryVariantNameFormulaPartCommand> ResolveParts(CreateCategoryVariantNameFormulaCommand command)
    {
        if (command.Parts.Count > 0)
        {
            return command.Parts
                .Where(x => x.AttributeRef != Guid.Empty)
                .GroupBy(x => x.AttributeRef)
                .Select(x => x.First())
                .OrderBy(x => x.SortOrder)
                .Select((part, index) => new CategoryVariantNameFormulaPartCommand
                {
                    AttributeRef = part.AttributeRef,
                    Separator = part.Separator,
                    SortOrder = index + 1
                })
                .ToList();
        }

        return (command.AttributeRefs ?? new List<Guid>())
            .Where(x => x != Guid.Empty)
            .Distinct()
            .Select((attributeRef, index) => new CategoryVariantNameFormulaPartCommand
            {
                AttributeRef = attributeRef,
                Separator = command.Separator,
                SortOrder = index + 1
            })
            .ToList();
    }
}

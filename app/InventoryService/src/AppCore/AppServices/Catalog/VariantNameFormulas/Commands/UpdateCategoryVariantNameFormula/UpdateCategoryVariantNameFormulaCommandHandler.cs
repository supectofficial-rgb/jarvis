namespace Insurance.InventoryService.AppCore.AppServices.Catalog.VariantNameFormulas.Commands.UpdateCategoryVariantNameFormula;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands.UpdateCategoryVariantNameFormula;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateCategoryVariantNameFormulaCommandHandler
    : CommandHandler<UpdateCategoryVariantNameFormulaCommand, UpdateCategoryVariantNameFormulaCommandResult>
{
    private readonly ICategoryVariantNameFormulaCommandRepository _repository;
    private readonly ICategoryQueryRepository _categoryQueryRepository;

    public UpdateCategoryVariantNameFormulaCommandHandler(
        ICategoryVariantNameFormulaCommandRepository repository,
        ICategoryQueryRepository categoryQueryRepository)
    {
        _repository = repository;
        _categoryQueryRepository = categoryQueryRepository;
    }

    public override async Task<CommandResult<UpdateCategoryVariantNameFormulaCommandResult>> Handle(UpdateCategoryVariantNameFormulaCommand command)
    {
        if (command.FormulaBusinessKey == Guid.Empty)
            return Fail("FormulaBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        if (command.AttributeRefs is null || command.AttributeRefs.Count == 0)
            return Fail("At least one formula attribute is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.FormulaBusinessKey);
        if (aggregate is null)
            return Fail("Formula was not found.");

        var normalizedName = command.Name.Trim();
        if (await _repository.ExistsByCategoryAndNameAsync(aggregate.CategoryRef, normalizedName, command.FormulaBusinessKey))
            return Fail($"Formula '{normalizedName}' already exists for this category.");

        var allowedAttributes = await _categoryQueryRepository.GetCategoryAttributeRulesByCategoryIdAsync(aggregate.CategoryRef, includeInherited: true, includeInactive: false);
        var allowedAttributeRefs = allowedAttributes.Select(x => x.AttributeRef).ToHashSet();
        if (command.AttributeRefs.Any(x => !allowedAttributeRefs.Contains(x)))
            return Fail("Formula contains attributes that are not assigned to the selected category.");

        try
        {
            aggregate.Update(normalizedName, command.Separator, command.DisplayOrder, command.AttributeRefs, command.IsActive);
            await _repository.CommitAsync();
            return Ok(new UpdateCategoryVariantNameFormulaCommandResult { FormulaBusinessKey = aggregate.BusinessKey.Value });
        }
        catch (Exception ex)
        {
            return Fail($"Updating variant name formula failed: {ex.Message}");
        }
    }
}

namespace Insurance.InventoryService.AppCore.AppServices.Catalog.VariantNameFormulas.Commands.DeleteCategoryVariantNameFormula;

using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands.DeleteCategoryVariantNameFormula;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteCategoryVariantNameFormulaCommandHandler
    : CommandHandler<DeleteCategoryVariantNameFormulaCommand, DeleteCategoryVariantNameFormulaCommandResult>
{
    private readonly ICategoryVariantNameFormulaCommandRepository _repository;

    public DeleteCategoryVariantNameFormulaCommandHandler(ICategoryVariantNameFormulaCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<DeleteCategoryVariantNameFormulaCommandResult>> Handle(DeleteCategoryVariantNameFormulaCommand command)
    {
        if (command.FormulaBusinessKey == Guid.Empty)
            return Fail("FormulaBusinessKey is required.");

        var deleted = await _repository.DeleteByBusinessKeyAsync(command.FormulaBusinessKey);
        if (deleted == 0)
            return Fail("Formula was not found.");

        return Ok(new DeleteCategoryVariantNameFormulaCommandResult { FormulaBusinessKey = command.FormulaBusinessKey });
    }
}

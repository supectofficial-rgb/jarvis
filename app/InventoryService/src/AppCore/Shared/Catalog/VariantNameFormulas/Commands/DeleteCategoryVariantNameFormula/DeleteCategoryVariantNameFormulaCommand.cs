namespace Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Commands.DeleteCategoryVariantNameFormula;

using OysterFx.AppCore.Shared.Commands;

public class DeleteCategoryVariantNameFormulaCommand : ICommand<DeleteCategoryVariantNameFormulaCommandResult>
{
    public Guid FormulaBusinessKey { get; set; }
}

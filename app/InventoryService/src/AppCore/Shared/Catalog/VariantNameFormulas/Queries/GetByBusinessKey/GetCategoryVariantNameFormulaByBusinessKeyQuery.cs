namespace Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetCategoryVariantNameFormulaByBusinessKeyQuery : IQuery<GetCategoryVariantNameFormulaByBusinessKeyQueryResult>
{
    public GetCategoryVariantNameFormulaByBusinessKeyQuery(Guid formulaBusinessKey)
    {
        FormulaBusinessKey = formulaBusinessKey;
    }

    public Guid FormulaBusinessKey { get; }
}

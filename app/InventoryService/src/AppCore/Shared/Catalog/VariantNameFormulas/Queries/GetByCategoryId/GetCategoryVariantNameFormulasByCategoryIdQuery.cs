namespace Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries.GetByCategoryId;

using OysterFx.AppCore.Shared.Queries;

public class GetCategoryVariantNameFormulasByCategoryIdQuery : IQuery<GetCategoryVariantNameFormulasByCategoryIdQueryResult>
{
    public GetCategoryVariantNameFormulasByCategoryIdQuery(Guid categoryRef, bool includeInactive = true)
    {
        CategoryRef = categoryRef;
        IncludeInactive = includeInactive;
    }

    public Guid CategoryRef { get; }
    public bool IncludeInactive { get; }
}

namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetCategoryByBusinessKeyQuery : IQuery<GetCategoryByBusinessKeyQueryResult>
{
    public GetCategoryByBusinessKeyQuery(Guid categoryBusinessKey)
    {
        CategoryBusinessKey = categoryBusinessKey;
    }

    public Guid CategoryBusinessKey { get; }
}

namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetByCategoryId;

using OysterFx.AppCore.Shared.Queries;

public class GetProductsByCategoryIdQuery : IQuery<GetProductsByCategoryIdQueryResult>
{
    public GetProductsByCategoryIdQuery(Guid categoryId, bool includeInactive = false)
    {
        CategoryId = categoryId;
        IncludeInactive = includeInactive;
    }

    public Guid CategoryId { get; }
    public bool IncludeInactive { get; }
}

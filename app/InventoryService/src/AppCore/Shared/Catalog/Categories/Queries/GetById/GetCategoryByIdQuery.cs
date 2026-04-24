namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public class GetCategoryByIdQuery : IQuery<GetCategoryByBusinessKeyQueryResult>
{
    public GetCategoryByIdQuery(Guid categoryId)
    {
        CategoryId = categoryId;
    }

    public Guid CategoryId { get; }
}

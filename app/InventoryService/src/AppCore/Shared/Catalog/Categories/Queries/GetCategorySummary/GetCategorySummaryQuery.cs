namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategorySummary;

using OysterFx.AppCore.Shared.Queries;

public class GetCategorySummaryQuery : IQuery<GetCategorySummaryQueryResult>
{
    public GetCategorySummaryQuery(Guid categoryId)
    {
        CategoryId = categoryId;
    }

    public Guid CategoryId { get; }
}

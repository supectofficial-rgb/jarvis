namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.SearchProducts;

using OysterFx.AppCore.Shared.Queries;

public class SearchProductsQuery : IQuery<SearchProductsQueryResult>
{
    public Guid? CategoryRef { get; set; }
    public string? BaseSku { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

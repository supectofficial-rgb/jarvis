namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.SearchSerialItems;

using OysterFx.AppCore.Shared.Queries;

public class SearchSerialItemsQuery : IQuery<SearchSerialItemsQueryResult>
{
    public string? SerialNo { get; set; }
    public Guid? VariantRef { get; set; }
    public Guid? SellerRef { get; set; }
    public Guid? WarehouseRef { get; set; }
    public Guid? LocationRef { get; set; }
    public Guid? StockDetailRef { get; set; }
    public Guid? QualityStatusRef { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

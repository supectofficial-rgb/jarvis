namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.SearchStockDetails;

using OysterFx.AppCore.Shared.Queries;

public class SearchStockDetailsQuery : IQuery<SearchStockDetailsQueryResult>
{
    public Guid? VariantRef { get; set; }
    public Guid? SellerRef { get; set; }
    public Guid? WarehouseRef { get; set; }
    public Guid? LocationRef { get; set; }
    public Guid? QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public bool? IsEmpty { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

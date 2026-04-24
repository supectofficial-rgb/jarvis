namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.SearchInventoryDocuments;

using OysterFx.AppCore.Shared.Queries;

public class SearchInventoryDocumentsQuery : IQuery<SearchInventoryDocumentsQueryResult>
{
    public string? DocumentNo { get; set; }
    public string? DocumentType { get; set; }
    public string? Status { get; set; }
    public Guid? WarehouseRef { get; set; }
    public Guid? SellerRef { get; set; }
    public DateTime? OccurredFrom { get; set; }
    public DateTime? OccurredTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

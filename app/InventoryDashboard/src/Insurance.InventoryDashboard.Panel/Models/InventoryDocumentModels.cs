using System.ComponentModel.DataAnnotations;

namespace Insurance.InventoryDashboard.Panel.Models;

public sealed class InventoryDocumentManagementPageViewModel
{
    public string UserName { get; set; } = "کاربر";
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
    public IReadOnlyList<DashboardMenuModule> Modules { get; set; } = Array.Empty<DashboardMenuModule>();
    public DashboardMenuModule? ActiveModule { get; set; }
    public DashboardMenuItem? ActiveItem { get; set; }
    public string? ErrorMessage { get; set; }

    public IReadOnlyList<WarehouseLookupItemModel> Warehouses { get; set; } = Array.Empty<WarehouseLookupItemModel>();
    public IReadOnlyList<LocationListItemModel> Locations { get; set; } = Array.Empty<LocationListItemModel>();
    public IReadOnlyList<SellerLookupModel> Sellers { get; set; } = Array.Empty<SellerLookupModel>();
    public IReadOnlyList<QualityStatusLookupModel> QualityStatuses { get; set; } = Array.Empty<QualityStatusLookupModel>();
    public ProductVariantSearchResultModel VariantSearchResult { get; set; } = new();
    public InventoryDocumentSearchResultModel Documents { get; set; } = new();

    public InventoryDocumentForm DocumentForm { get; set; } = new();
    public string? VariantSearchTerm { get; set; }
    public string? DocumentNoFilter { get; set; }
    public string? DocumentTypeFilter { get; set; }
    public string? StatusFilter { get; set; }
    public string? WarehouseFilterId { get; set; }
    public int DocumentPage { get; set; } = 1;
    public int DocumentPageSize { get; set; } = 10;
    public IReadOnlyList<int> PageSizeOptions { get; set; } = new[] { 10, 25, 50 };
}

public sealed class InventoryDocumentForm
{
    public string? DocumentNo { get; set; }
    public string DocumentType { get; set; } = "Receipt";

    [Required]
    public string WarehouseId { get; set; } = string.Empty;

    [Required]
    public string SellerId { get; set; } = string.Empty;

    public DateTime OccurredAt { get; set; } = DateTime.Today;
    public string? ReasonCode { get; set; }
    public string LinesJson { get; set; } = "[]";
    public bool PostAfterCreate { get; set; }
}

public sealed class InventoryDocumentLineForm
{
    public string VariantId { get; set; } = string.Empty;
    public string VariantSku { get; set; } = string.Empty;
    public string BaseUomRef { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? SourceLocationId { get; set; }
    public string? DestinationLocationId { get; set; }
    public string? QualityStatusId { get; set; }
    public string? AdjustmentDirection { get; set; }
    public string? LotBatchNo { get; set; }
    public string? ReasonCode { get; set; }
}

public sealed class InventoryDocumentSearchResultModel
{
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<InventoryDocumentListItemModel> Items { get; set; } = new();
}

public sealed class InventoryDocumentListItemModel
{
    public Guid DocumentBusinessKey { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid WarehouseRef { get; set; }
    public Guid SellerRef { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? PostedAt { get; set; }
    public Guid? PostedTransactionRef { get; set; }
    public string? ReasonCode { get; set; }
}

public sealed class InventoryDocumentLineItemModel
{
    public Guid LineBusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public decimal Qty { get; set; }
    public Guid UomRef { get; set; }
    public decimal BaseQty { get; set; }
    public Guid BaseUomRef { get; set; }
    public Guid? SourceLocationRef { get; set; }
    public Guid? DestinationLocationRef { get; set; }
    public Guid? QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public string? ReasonCode { get; set; }
    public string? AdjustmentDirection { get; set; }
}

public sealed class QualityStatusLookupResultModel
{
    public List<QualityStatusLookupModel> Items { get; set; } = new();
}

public sealed class QualityStatusLookupModel
{
    public Guid QualityStatusBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

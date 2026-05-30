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

    public string? SelectedDocumentId { get; set; }
    public string? EditingLineId { get; set; }
    public string? SelectedTab { get; set; }
    public InventoryDocumentDetailsModel? SelectedDocumentDetails { get; set; }
    public IReadOnlyList<InventoryDocumentListItemModel> Documents { get; set; } = Array.Empty<InventoryDocumentListItemModel>();
    public IReadOnlyList<WarehouseLookupItemModel> WarehouseLookup { get; set; } = Array.Empty<WarehouseLookupItemModel>();
    public IReadOnlyList<SellerLookupItemModel> SellerLookup { get; set; } = Array.Empty<SellerLookupItemModel>();
    public IReadOnlyList<LocationLookupItemModel> LocationLookup { get; set; } = Array.Empty<LocationLookupItemModel>();
    public IReadOnlyList<QualityStatusLookupItemModel> QualityStatusLookup { get; set; } = Array.Empty<QualityStatusLookupItemModel>();
    public IReadOnlyList<UserSummaryModel> UserLookup { get; set; } = Array.Empty<UserSummaryModel>();
    public IReadOnlyList<RoleSummaryModel> RoleLookup { get; set; } = Array.Empty<RoleSummaryModel>();
    public IReadOnlyList<ProductSummaryModel> ProductLookup { get; set; } = Array.Empty<ProductSummaryModel>();
    public IReadOnlyList<ProductVariantSummaryModel> VariantLookup { get; set; } = Array.Empty<ProductVariantSummaryModel>();
    public IReadOnlyList<UnitOfMeasureLookupModel> UnitOfMeasureLookup { get; set; } = Array.Empty<UnitOfMeasureLookupModel>();

    public string? DocumentNoFilter { get; set; }
    public string? DocumentTypeFilter { get; set; }
    public string? DocumentStatusFilter { get; set; }
    public string? VariantFilter { get; set; }
    public string? WarehouseFilter { get; set; }
    public string? LocationFilter { get; set; }
    public string? SellerFilter { get; set; }
    public string? OccurredFromFilter { get; set; }
    public string? OccurredToFilter { get; set; }

    public int DocumentPage { get; set; } = 1;
    public int DocumentPageSize { get; set; } = 10;
    public int DocumentTotalCount { get; set; }
    public int DocumentTotalPages { get; set; } = 1;
    public IReadOnlyList<int> PageSizeOptions { get; set; } = new[] { 10, 25, 50 };

    public string? ErrorMessage { get; set; }
    public CreateInventoryDocumentForm CreateForm { get; set; } = new();
    public InventoryDocumentLineForm LineForm { get; set; } = new();
}

public sealed class InventoryDocumentListItemModel
{
    public string DocumentBusinessKey { get; set; } = string.Empty;
    public string DocumentNo { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReferenceType { get; set; }
    public string? ReferenceBusinessId { get; set; }
    public string WarehouseRef { get; set; } = string.Empty;
    public string SellerRef { get; set; } = string.Empty;
    public string? ReceivedBy { get; set; }
    public string? DeliveredBy { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? PostedTransactionRef { get; set; }
    public string? ReasonCode { get; set; }
}

public sealed class InventoryDocumentLineDetailsModel
{
    public string LineBusinessKey { get; set; } = string.Empty;
    public string VariantRef { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string UomRef { get; set; } = string.Empty;
    public decimal BaseQty { get; set; }
    public string BaseUomRef { get; set; } = string.Empty;
    public string? SourceLocationRef { get; set; }
    public string? DestinationLocationRef { get; set; }
    public string? QualityStatusRef { get; set; }
    public string? FromQualityStatusRef { get; set; }
    public string? ToQualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public string? ReasonCode { get; set; }
    public string? AdjustmentDirection { get; set; }
    public List<InventoryDocumentLineSerialModel> Serials { get; set; } = new();
}

public sealed class InventoryDocumentLineSerialModel
{
    public string SerialItemBusinessKey { get; set; } = string.Empty;
    public string? SerialRef { get; set; }
    public string SerialNo { get; set; } = string.Empty;
}

public sealed class SerialItemLookupModel
{
    public string SerialItemBusinessKey { get; set; } = string.Empty;
    public string SerialNo { get; set; } = string.Empty;
    public string VariantRef { get; set; } = string.Empty;
    public string SellerRef { get; set; } = string.Empty;
    public string WarehouseRef { get; set; } = string.Empty;
    public string LocationRef { get; set; } = string.Empty;
    public string? StockDetailRef { get; set; }
    public string QualityStatusRef { get; set; } = string.Empty;
    public string? LotBatchNo { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DateScannedIn { get; set; }
    public string? LastTransactionRef { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}

public sealed class PostDocumentSerialItemModel
{
    public string SerialItemBusinessKey { get; set; } = string.Empty;
    public string SerialNo { get; set; } = string.Empty;
}

public sealed class PostDocumentLineSerialSelectionModel
{
    public string DocumentLineBusinessKey { get; set; } = string.Empty;
    public bool UseUniqueSerialItems { get; set; }
    public List<PostDocumentSerialItemModel> Serials { get; set; } = new();
}

public sealed class InventoryDocumentDetailsModel
{
    public string DocumentBusinessKey { get; set; } = string.Empty;
    public string DocumentNo { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReferenceType { get; set; }
    public string? ReferenceBusinessId { get; set; }
    public string WarehouseRef { get; set; } = string.Empty;
    public string SellerRef { get; set; } = string.Empty;
    public string? ReceivedBy { get; set; }
    public string? DeliveredBy { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PostedAt { get; set; }
    public string? PostedTransactionRef { get; set; }
    public string? ReasonCode { get; set; }
    public List<InventoryDocumentLineDetailsModel> Lines { get; set; } = new();
}

public sealed class InventoryDocumentSearchResultModel
{
    public int TotalCount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<InventoryDocumentListItemModel> Items { get; set; } = new();
}

public sealed class SellerLookupItemModel
{
    public string SellerBusinessKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class LocationLookupItemModel
{
    public string LocationBusinessKey { get; set; } = string.Empty;
    public string WarehouseRef { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
}

public sealed class QualityStatusLookupItemModel
{
    public string QualityStatusBusinessKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class InventoryDocumentDecisionForm
{
    [Required]
    public string DocumentId { get; set; } = string.Empty;

    [StringLength(120)]
    public string? ReasonCode { get; set; }
}

public sealed class InventoryDocumentStatusChangeForm
{
    [Required]
    public string DocumentId { get; set; } = string.Empty;

    [Required]
    public string Action { get; set; } = string.Empty;

    [StringLength(120)]
    public string? ReasonCode { get; set; }
}

public sealed class CreateInventoryDocumentForm
{
    public string? DocumentId { get; set; }

    [Required(ErrorMessage = "نوع سند الزامی است.")]
    public string DocumentType { get; set; } = "Receipt";

    [StringLength(64)]
    public string? DocumentNo { get; set; }

    [StringLength(80)]
    public string? ReferenceType { get; set; }

    public string? ReferenceBusinessId { get; set; }

    public string WarehouseRef { get; set; } = string.Empty;

    public string SellerRef { get; set; } = string.Empty;

    public string? ReceivedBy { get; set; }

    public string? DeliveredBy { get; set; }

    [Required(ErrorMessage = "زمان وقوع الزامی است.")]
    public DateTime OccurredAt { get; set; } = DateTime.Now;

    [StringLength(120)]
    public string? ReasonCode { get; set; }

    public string? ConversionVariantId { get; set; }

    public string ConversionOperationType { get; set; } = "Assemble";

    public decimal ConversionQuantity { get; set; } = 1m;

    public List<CreateInventoryDocumentLineForm> Lines { get; set; } = new();
}

public sealed class CreateInventoryDocumentLineForm
{
    [Required(ErrorMessage = "انتخاب واریانت الزامی است.")]
    public string VariantId { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.000001", "999999999", ErrorMessage = "مقدار باید بزرگ‌تر از صفر باشد.")]
    public decimal Qty { get; set; } = 1m;

    public string? UomRef { get; set; }
    public string? BaseUomRef { get; set; }

    public string? SourceLocationRef { get; set; }
    public string? DestinationLocationRef { get; set; }
    public string? QualityStatusRef { get; set; }
    public string? FromQualityStatusRef { get; set; }
    public string? ToQualityStatusRef { get; set; }

    [StringLength(120)]
    public string? LotBatchNo { get; set; }

    [StringLength(120)]
    public string? ReasonCode { get; set; }

    public string? AdjustmentDirection { get; set; }
    public List<InventoryDocumentLineSerialModel> Serials { get; set; } = new();
}

public sealed class CreateDocumentUserForm
{
    [Required(ErrorMessage = "نام کاربر الزامی است.")]
    [StringLength(150)]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "شماره تلفن الزامی است.")]
    [StringLength(32)]
    public string MobileNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "نقش الزامی است.")]
    public string RoleBusinessKey { get; set; } = string.Empty;
}

public sealed class InventoryDocumentLineForm
{
    [Required]
    public string DocumentId { get; set; } = string.Empty;

    [Required]
    public string DocumentType { get; set; } = "Receipt";

    public string? LineId { get; set; }

    [Required(ErrorMessage = "انتخاب واریانت الزامی است.")]
    public string VariantId { get; set; } = string.Empty;

    public string? WarehouseRef { get; set; }

    [Range(typeof(decimal), "0.000001", "999999999", ErrorMessage = "مقدار باید بزرگ‌تر از صفر باشد.")]
    public decimal Qty { get; set; } = 1m;

    public string? UomRef { get; set; }
    public string? BaseUomRef { get; set; }
    public string? SourceLocationRef { get; set; }
    public string? DestinationLocationRef { get; set; }
    public string? QualityStatusRef { get; set; }
    public string? FromQualityStatusRef { get; set; }
    public string? ToQualityStatusRef { get; set; }

    [StringLength(120)]
    public string? LotBatchNo { get; set; }

    [StringLength(120)]
    public string? ReasonCode { get; set; }

    public string? AdjustmentDirection { get; set; }
    public bool UseUniqueSerialItems { get; set; }
    public List<InventoryDocumentLineSerialModel> Serials { get; set; } = new();
}

public sealed class CreateInventoryDocumentResultModel
{
    public string DocumentId { get; set; } = string.Empty;
    public string DocumentNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public sealed class ConversionDocumentPreviewViewModel
{
    public string DocumentId { get; set; } = string.Empty;
    public string DocumentNo { get; set; } = string.Empty;
    public string VariantId { get; set; } = string.Empty;
    public string VariantLabel { get; set; } = string.Empty;
    public string OperationType { get; set; } = "Assemble";
    public decimal Quantity { get; set; } = 1m;
    public string? ReasonCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SelectedSellerQualityKey { get; set; }
    public List<ConversionDocumentPreviewLineModel> Lines { get; set; } = new();
}

public sealed class ConversionDocumentPreviewLineModel
{
    public string LineKey { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string VariantId { get; set; } = string.Empty;
    public string VariantLabel { get; set; } = string.Empty;
    public string BaseUomRef { get; set; } = string.Empty;
    public string BaseUomLabel { get; set; } = string.Empty;
    public decimal RequiredQty { get; set; }
    public string? SelectedLocationRef { get; set; }
    public List<ConversionDocumentPreviewLocationModel> LocationOptions { get; set; } = new();
}

public sealed class ConversionDocumentPreviewLocationModel
{
    public string LocationRef { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public decimal QuantityOnHand { get; set; }
    public bool IsDefault { get; set; }
}

public sealed class ConversionDocumentPreviewSelectionModel
{
    public string LineKey { get; set; } = string.Empty;
    public string VariantId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string LocationRef { get; set; } = string.Empty;
    public decimal RequiredQty { get; set; }
}

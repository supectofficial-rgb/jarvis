namespace Insurance.InventoryDashboard.Panel.Models;

public sealed class PricingPageViewModel
{
    public string UserName { get; set; } = "کاربر";
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public IReadOnlyList<DashboardMenuModule> Modules { get; set; } = Array.Empty<DashboardMenuModule>();
    public DashboardMenuModule? ActiveModule { get; set; }
    public DashboardMenuItem? ActiveItem { get; set; }
    public PriceTypeSearchResultModel PriceTypes { get; set; } = new();
    public PriceChannelSearchResultModel PriceChannels { get; set; } = new();
    public SellerVariantPriceSearchResultModel VariantPrices { get; set; } = new();
    public ProductVariantSearchResultModel VariantSearchResult { get; set; } = new();
    public List<PriceTypeLookupModel> PriceTypeLookup { get; set; } = new();
    public List<PriceChannelLookupModel> PriceChannelLookup { get; set; } = new();
    public List<SellerLookupModel> Sellers { get; set; } = new();
    public SellerSearchItemModel? OwnerSeller { get; set; }
    public List<StockDetailBucketModel> AvailableBuckets { get; set; } = new();
    public BulkVariantPricingRequest BulkPricingForm { get; set; } = new();
    public string? VariantSearchTerm { get; set; }
    public string? StatusMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

public sealed class PriceTypeSearchResultModel
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<PriceTypeModel> Items { get; set; } = new();
}

public sealed class PriceTypeModel
{
    public Guid PriceTypeBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class PriceTypeLookupResultModel
{
    public List<PriceTypeLookupModel> Items { get; set; } = new();
}

public sealed class PriceTypeLookupModel
{
    public Guid PriceTypeBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class PriceChannelSearchResultModel
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<PriceChannelModel> Items { get; set; } = new();
}

public sealed class PriceChannelModel
{
    public Guid PriceChannelBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class PriceChannelLookupResultModel
{
    public List<PriceChannelLookupModel> Items { get; set; } = new();
}

public sealed class PriceChannelLookupModel
{
    public Guid PriceChannelBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class SellerVariantPriceSearchResultModel
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<SellerVariantPriceModel> Items { get; set; } = new();
}

public sealed class SellerVariantPriceModel
{
    public Guid SellerVariantPriceBusinessKey { get; set; }
    public Guid SellerRef { get; set; }
    public Guid VariantRef { get; set; }
    public Guid PriceTypeRef { get; set; }
    public Guid PriceChannelRef { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal MinQty { get; set; }
    public int Priority { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
}

public sealed class SellerLookupResultModel
{
    public List<SellerLookupModel> Items { get; set; } = new();
}

public sealed class SellerSearchResultModel
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<SellerSearchItemModel> Items { get; set; } = new();
}

public sealed class SellerSearchItemModel
{
    public Guid SellerBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsSystemOwner { get; set; }
    public bool IsActive { get; set; }
}

public sealed class SellerLookupModel
{
    public Guid SellerBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class StockDetailBucketResultModel
{
    public List<StockDetailBucketModel> Items { get; set; } = new();
}

public sealed class StockDetailBucketModel
{
    public Guid StockDetailBusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public decimal QuantityOnHand { get; set; }
}

public sealed class UpsertPriceTypeRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class UpsertPriceChannelRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class UpsertSellerVariantPriceRequest
{
    public Guid SellerRef { get; set; }
    public Guid VariantRef { get; set; }
    public Guid PriceTypeRef { get; set; }
    public Guid PriceChannelRef { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IRR";
    public decimal MinQty { get; set; } = 1;
    public int Priority { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public List<SellerVariantPriceOfferInputModel> Offers { get; set; } = new();
}

public sealed class SellerVariantPriceOfferInputModel
{
    public string Name { get; set; } = string.Empty;
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? MaxQuantity { get; set; }
    public int Priority { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class BulkVariantPricingRequest
{
    public List<string> SelectedVariantRefs { get; set; } = new();
    public decimal? ApplyToAllAmount { get; set; }
    public string Currency { get; set; } = "IRR";
    public decimal MinQty { get; set; } = 1;
    public int Priority { get; set; }
    public List<VariantPriceMatrixInputModel> Prices { get; set; } = new();
}

public sealed class VariantPriceMatrixInputModel
{
    public Guid PriceTypeRef { get; set; }
    public Guid PriceChannelRef { get; set; }
    public decimal? Amount { get; set; }
}

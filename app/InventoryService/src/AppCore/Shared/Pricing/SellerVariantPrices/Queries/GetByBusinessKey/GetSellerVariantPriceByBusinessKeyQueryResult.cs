namespace Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.GetByBusinessKey;

public class GetSellerVariantPriceByBusinessKeyQueryResult
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
    public List<SellerVariantPriceOfferResultItem> Offers { get; set; } = new();
}

public class SellerVariantPriceOfferResultItem
{
    public string Name { get; set; } = string.Empty;
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? MaxQuantity { get; set; }
    public int Priority { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public bool IsActive { get; set; }
}

namespace Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands;

public class SellerVariantPriceOfferItem
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

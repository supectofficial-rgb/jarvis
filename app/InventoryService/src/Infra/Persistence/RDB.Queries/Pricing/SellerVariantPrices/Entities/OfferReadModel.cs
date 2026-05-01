namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.SellerVariantPrices.Entities;

public class OfferReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid PriceRef { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? MaxQuantity { get; set; }
    public int Priority { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public bool IsActive { get; set; }
}

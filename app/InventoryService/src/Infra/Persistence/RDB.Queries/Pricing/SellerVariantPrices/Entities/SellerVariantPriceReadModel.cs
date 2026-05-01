namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.SellerVariantPrices.Entities;

public class SellerVariantPriceReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
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

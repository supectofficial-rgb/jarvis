namespace Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands.CreateSellerVariantPrice;

public class CreateSellerVariantPriceCommandResult
{
    public Guid SellerVariantPriceBusinessKey { get; set; }
    public Guid SellerRef { get; set; }
    public Guid VariantRef { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

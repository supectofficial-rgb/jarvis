namespace Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands.UpdateSellerVariantPrice;

public class UpdateSellerVariantPriceCommandResult
{
    public Guid SellerVariantPriceBusinessKey { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

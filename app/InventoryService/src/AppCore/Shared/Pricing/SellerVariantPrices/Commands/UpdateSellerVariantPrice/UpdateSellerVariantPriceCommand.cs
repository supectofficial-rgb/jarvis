namespace Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands.UpdateSellerVariantPrice;

using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands;
using OysterFx.AppCore.Shared.Commands;

public class UpdateSellerVariantPriceCommand : ICommand<UpdateSellerVariantPriceCommandResult>
{
    public Guid SellerVariantPriceBusinessKey { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal MinQty { get; set; }
    public int Priority { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public List<SellerVariantPriceOfferItem> Offers { get; set; } = new();
}

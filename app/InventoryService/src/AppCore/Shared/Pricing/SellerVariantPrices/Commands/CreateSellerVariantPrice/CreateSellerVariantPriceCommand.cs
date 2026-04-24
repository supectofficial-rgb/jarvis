namespace Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands.CreateSellerVariantPrice;

using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands;
using OysterFx.AppCore.Shared.Commands;

public class CreateSellerVariantPriceCommand : ICommand<CreateSellerVariantPriceCommandResult>
{
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
    public List<SellerVariantPriceOfferItem> Offers { get; set; } = new();
}

namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ChangeVariantTrackingPolicy;

public class ChangeVariantTrackingPolicyCommandResult
{
    public Guid ProductVariantBusinessKey { get; set; }
    public string TrackingPolicy { get; set; } = string.Empty;
}

namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ChangeVariantTrackingPolicy;

using OysterFx.AppCore.Shared.Commands;

public class ChangeVariantTrackingPolicyCommand : ICommand<ChangeVariantTrackingPolicyCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public string TrackingPolicy { get; set; } = string.Empty;
}

namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.ChangeVariantTrackingPolicy;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ChangeVariantTrackingPolicy;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ChangeProductVariantTrackingPolicyCommandHandler : CommandHandler<ChangeVariantTrackingPolicyCommand, ChangeVariantTrackingPolicyCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public ChangeProductVariantTrackingPolicyCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<ChangeVariantTrackingPolicyCommandResult>> Handle(ChangeVariantTrackingPolicyCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (!Enum.TryParse<TrackingPolicy>(command.TrackingPolicy, true, out var trackingPolicy))
            return Fail($"Unsupported tracking policy '{command.TrackingPolicy}'.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        try
        {
            variant.ChangeTrackingPolicy(trackingPolicy);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _variantRepository.CommitAsync();

        return Ok(new ChangeVariantTrackingPolicyCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            TrackingPolicy = variant.TrackingPolicy.ToString()
        });
    }
}




namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.DeactivateVariant;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.DeactivateVariant;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeactivateProductVariantCommandHandler : CommandHandler<DeactivateVariantCommand, DeactivateVariantCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public DeactivateProductVariantCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<DeactivateVariantCommandResult>> Handle(DeactivateVariantCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        variant.Deactivate();
        await _variantRepository.CommitAsync();

        return Ok(new DeactivateVariantCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            IsActive = variant.IsActive
        });
    }
}




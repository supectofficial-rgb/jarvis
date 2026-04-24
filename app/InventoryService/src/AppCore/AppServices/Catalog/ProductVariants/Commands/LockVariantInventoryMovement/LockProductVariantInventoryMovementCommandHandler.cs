namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.LockVariantInventoryMovement;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.LockVariantInventoryMovement;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class LockProductVariantInventoryMovementCommandHandler : CommandHandler<LockVariantInventoryMovementCommand, LockVariantInventoryMovementCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public LockProductVariantInventoryMovementCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<LockVariantInventoryMovementCommandResult>> Handle(LockVariantInventoryMovementCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        if (!variant.InventoryMovementLocked)
        {
            variant.MarkInventoryMovementStarted();
            await _variantRepository.CommitAsync();
        }

        return Ok(new LockVariantInventoryMovementCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            InventoryMovementLocked = variant.InventoryMovementLocked
        });
    }
}




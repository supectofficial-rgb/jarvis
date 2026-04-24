namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.DeleteVariant;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.DeleteVariant;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteProductVariantCommandHandler : CommandHandler<DeleteVariantCommand, DeleteVariantCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public DeleteProductVariantCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<DeleteVariantCommandResult>> Handle(DeleteVariantCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        if (variant.InventoryMovementLocked)
            return Fail("Variant cannot be deleted after inventory movement has started.");

        variant.Deactivate();
        await _variantRepository.CommitAsync();

        return Ok(new DeleteVariantCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            Deleted = true
        });
    }
}




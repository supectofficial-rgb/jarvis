namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.RemoveVariantImage;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantImage;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RemoveVariantImageCommandHandler : CommandHandler<RemoveVariantImageCommand, RemoveVariantImageCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public RemoveVariantImageCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<RemoveVariantImageCommandResult>> Handle(RemoveVariantImageCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.FileKey))
            return Fail("FileKey is required.");

        var variant = await _variantRepository.GetByImageFileKeyAsync(command.FileKey);
        if (variant is null)
            return Fail("Variant image was not found.");

        if (!variant.RemoveImage(command.FileKey))
            return Fail("Variant image was not found.");

        await _variantRepository.CommitAsync();

        return Ok(new RemoveVariantImageCommandResult
        {
            FileKey = command.FileKey.Trim()
        });
    }
}

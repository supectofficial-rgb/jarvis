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
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.FileKey))
            return Fail("FileKey is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        variant.RemoveImage(command.FileKey);
        await _variantRepository.CommitAsync();

        return Ok(new RemoveVariantImageCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            FileKey = command.FileKey.Trim()
        });
    }
}

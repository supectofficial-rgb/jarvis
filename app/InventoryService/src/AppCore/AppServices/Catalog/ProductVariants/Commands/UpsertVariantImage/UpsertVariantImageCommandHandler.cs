namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.UpsertVariantImage;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantImage;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpsertVariantImageCommandHandler : CommandHandler<UpsertVariantImageCommand, UpsertVariantImageCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public UpsertVariantImageCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<UpsertVariantImageCommandResult>> Handle(UpsertVariantImageCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.FileKey))
            return Fail("FileKey is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        try
        {
            variant.AddOrUpdateImage(
                command.FileKey,
                command.OriginalFileName,
                command.ContentType,
                command.OriginalUrl,
                command.ThumbnailUrl,
                command.DisplayOrder,
                command.IsPrimary);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _variantRepository.CommitAsync();

        return Ok(new UpsertVariantImageCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            FileKey = command.FileKey.Trim()
        });
    }
}

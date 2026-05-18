namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.UpsertVariantTag;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantTag;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpsertVariantTagCommandHandler : CommandHandler<UpsertVariantTagCommand, UpsertVariantTagCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public UpsertVariantTagCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<UpsertVariantTagCommandResult>> Handle(UpsertVariantTagCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.TagName))
            return Fail("TagName is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        var normalizedTagName = command.TagName.Trim();
        var tagExists = await _variantRepository.ExistsByTagNameAsync(
            command.ProductVariantBusinessKey,
            normalizedTagName,
            command.VariantTagBusinessKey);

        if (tagExists)
            return Fail("Tag already exists on this variant.");

        var tag = variant.AddOrUpdateTag(command.VariantTagBusinessKey, normalizedTagName, command.TagColor, command.DisplayOrder);
        await _variantRepository.CommitAsync();

        return Ok(new UpsertVariantTagCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            VariantTagBusinessKey = tag.BusinessKey.Value,
            TagName = tag.TagName,
            TagColor = tag.TagColor,
            DisplayOrder = tag.DisplayOrder
        });
    }
}

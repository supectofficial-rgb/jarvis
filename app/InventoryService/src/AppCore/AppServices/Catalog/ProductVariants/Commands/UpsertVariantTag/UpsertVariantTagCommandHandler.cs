namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.UpsertVariantTag;

using Insurance.InventoryService.AppCore.Shared.Catalog.Tags.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantTag;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpsertVariantTagCommandHandler : CommandHandler<UpsertVariantTagCommand, UpsertVariantTagCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;
    private readonly ITagCommandRepository _tagRepository;

    public UpsertVariantTagCommandHandler(
        IProductVariantCommandRepository variantRepository,
        ITagCommandRepository tagRepository)
    {
        _variantRepository = variantRepository;
        _tagRepository = tagRepository;
    }

    public override async Task<CommandResult<UpsertVariantTagCommandResult>> Handle(UpsertVariantTagCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (command.TagBusinessKey == Guid.Empty)
            return Fail("TagBusinessKey is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        var tag = await _tagRepository.GetByBusinessKeyAsync(command.TagBusinessKey);
        if (tag is null)
            return Fail("Tag was not found.");

        var tagExists = await _variantRepository.ExistsByTagRefAsync(
            command.ProductVariantBusinessKey,
            command.TagBusinessKey,
            command.VariantTagBusinessKey);

        if (tagExists)
            return Fail("Tag already exists on this variant.");

        var variantTag = variant.AddOrUpdateTag(
            command.VariantTagBusinessKey,
            tag.BusinessKey.Value,
            tag.TagName,
            tag.TagColor,
            command.DisplayOrder);
        await _variantRepository.CommitAsync();

        return Ok(new UpsertVariantTagCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            VariantTagBusinessKey = variantTag.BusinessKey.Value,
            TagBusinessKey = tag.BusinessKey.Value,
            TagName = variantTag.TagName,
            TagColor = variantTag.TagColor,
            DisplayOrder = variantTag.DisplayOrder
        });
    }
}

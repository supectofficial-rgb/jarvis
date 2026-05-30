namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.UpsertVariantAddOn;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Tags.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantAddOn;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpsertVariantAddOnCommandHandler : CommandHandler<UpsertVariantAddOnCommand, UpsertVariantAddOnCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;
    private readonly ITagCommandRepository _tagRepository;

    public UpsertVariantAddOnCommandHandler(
        IProductVariantCommandRepository variantRepository,
        ITagCommandRepository tagRepository)
    {
        _variantRepository = variantRepository;
        _tagRepository = tagRepository;
    }

    public override async Task<CommandResult<UpsertVariantAddOnCommandResult>> Handle(UpsertVariantAddOnCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        var hasVariantRef = command.AddOnVariantRef.HasValue && command.AddOnVariantRef.Value != Guid.Empty;
        var hasTagId = command.TagId.HasValue && command.TagId.Value != Guid.Empty;

        if (hasVariantRef == hasTagId)
            return Fail("Exactly one of AddOnVariantRef or TagId is required.");

        if (hasVariantRef && command.ProductVariantBusinessKey == command.AddOnVariantRef!.Value)
            return Fail("Variant cannot reference itself as an add-on.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        if (hasVariantRef)
        {
            var addOnExists = await _variantRepository.ExistsByBusinessKeyAsync(command.AddOnVariantRef!.Value, onlyActive: false);
            if (!addOnExists)
                return Fail("Add-on variant was not found.");
        }
        else
        {
            var tag = await _tagRepository.GetByBusinessKeyAsync(command.TagId!.Value);
            if (tag is null)
                return Fail("Tag was not found.");
        }

        var addOn = variant.AddOrUpdateAddOn(command.AddOnVariantRef, command.TagId, command.IsRequired);
        await _variantRepository.CommitAsync();

        return Ok(new UpsertVariantAddOnCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            AddOnVariantRef = addOn.AddOnVariantRef,
            TagId = addOn.TagId,
            IsRequired = addOn.IsRequired
        });
    }
}

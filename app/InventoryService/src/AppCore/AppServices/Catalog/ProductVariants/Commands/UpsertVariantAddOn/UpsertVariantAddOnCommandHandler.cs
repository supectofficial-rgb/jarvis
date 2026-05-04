namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.UpsertVariantAddOn;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantAddOn;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpsertVariantAddOnCommandHandler : CommandHandler<UpsertVariantAddOnCommand, UpsertVariantAddOnCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public UpsertVariantAddOnCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<UpsertVariantAddOnCommandResult>> Handle(UpsertVariantAddOnCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (command.AddOnVariantRef == Guid.Empty)
            return Fail("AddOnVariantRef is required.");

        if (command.ProductVariantBusinessKey == command.AddOnVariantRef)
            return Fail("Variant cannot reference itself as an add-on.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        var addOnExists = await _variantRepository.ExistsByBusinessKeyAsync(command.AddOnVariantRef, onlyActive: false);
        if (!addOnExists)
            return Fail("Add-on variant was not found.");

        variant.AddOrUpdateAddOn(command.AddOnVariantRef);
        await _variantRepository.CommitAsync();

        return Ok(new UpsertVariantAddOnCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            AddOnVariantRef = command.AddOnVariantRef
        });
    }
}

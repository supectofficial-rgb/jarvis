namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.RemoveVariantAddOn;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantAddOn;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RemoveVariantAddOnCommandHandler : CommandHandler<RemoveVariantAddOnCommand, RemoveVariantAddOnCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public RemoveVariantAddOnCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<RemoveVariantAddOnCommandResult>> Handle(RemoveVariantAddOnCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (command.AddOnVariantRef == Guid.Empty)
            return Fail("AddOnVariantRef is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        variant.RemoveAddOn(command.AddOnVariantRef);
        await _variantRepository.CommitAsync();

        return Ok(new RemoveVariantAddOnCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            AddOnVariantRef = command.AddOnVariantRef
        });
    }
}

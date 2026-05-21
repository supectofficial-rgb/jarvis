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
        if (command.VariantAddOnBusinessKey == Guid.Empty)
            return Fail("VariantAddOnBusinessKey is required.");

        var variant = await _variantRepository.GetByVariantAddOnBusinessKeyAsync(command.VariantAddOnBusinessKey);
        if (variant is null)
            return Fail("Variant add-on was not found.");

        if (!variant.RemoveAddOn(command.VariantAddOnBusinessKey))
            return Fail("Variant add-on was not found.");

        await _variantRepository.CommitAsync();

        return Ok(new RemoveVariantAddOnCommandResult
        {
            VariantAddOnBusinessKey = command.VariantAddOnBusinessKey
        });
    }
}

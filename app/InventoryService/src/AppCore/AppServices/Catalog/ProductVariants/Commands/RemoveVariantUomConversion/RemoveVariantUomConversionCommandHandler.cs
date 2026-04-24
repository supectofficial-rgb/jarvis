namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.RemoveVariantUomConversion;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantUomConversion;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RemoveVariantUomConversionCommandHandler : CommandHandler<RemoveVariantUomConversionCommand, RemoveVariantUomConversionCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public RemoveVariantUomConversionCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<RemoveVariantUomConversionCommandResult>> Handle(RemoveVariantUomConversionCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (command.FromUomRef == Guid.Empty)
            return Fail("FromUomRef is required.");

        if (command.ToUomRef == Guid.Empty)
            return Fail("ToUomRef is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        var existed = variant.UomConversions.Any(x => x.FromUomRef == command.FromUomRef && x.ToUomRef == command.ToUomRef);
        if (existed)
        {
            variant.RemoveConversion(command.FromUomRef, command.ToUomRef);
            await _variantRepository.CommitAsync();
        }

        return Ok(new RemoveVariantUomConversionCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            FromUomRef = command.FromUomRef,
            ToUomRef = command.ToUomRef,
            Removed = existed
        });
    }
}

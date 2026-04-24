namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.ChangeVariantBaseUom;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ChangeVariantBaseUom;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ChangeProductVariantBaseUomCommandHandler : CommandHandler<ChangeVariantBaseUomCommand, ChangeVariantBaseUomCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;
    private readonly IUnitOfMeasureCommandRepository _uomRepository;

    public ChangeProductVariantBaseUomCommandHandler(IProductVariantCommandRepository variantRepository, IUnitOfMeasureCommandRepository uomRepository)
    {
        _variantRepository = variantRepository;
        _uomRepository = uomRepository;
    }

    public override async Task<CommandResult<ChangeVariantBaseUomCommandResult>> Handle(ChangeVariantBaseUomCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (command.BaseUomRef == Guid.Empty)
            return Fail("BaseUomRef is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        var uom = await _uomRepository.GetByBusinessKeyAsync(command.BaseUomRef);
        if (uom is null)
            return Fail("Unit of measure was not found.");

        if (variant.IsActive && !uom.IsActive)
            return Fail("Active variant must have an active base UOM.");

        try
        {
            variant.ChangeBaseUom(command.BaseUomRef);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _variantRepository.CommitAsync();

        return Ok(new ChangeVariantBaseUomCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            BaseUomRef = variant.BaseUomRef
        });
    }
}




namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.UpsertVariantUomConversion;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantUomConversion;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpsertVariantUomConversionCommandHandler : CommandHandler<UpsertVariantUomConversionCommand, UpsertVariantUomConversionCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public UpsertVariantUomConversionCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<UpsertVariantUomConversionCommandResult>> Handle(UpsertVariantUomConversionCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (command.FromUomRef == Guid.Empty)
            return Fail("FromUomRef is required.");

        if (command.ToUomRef == Guid.Empty)
            return Fail("ToUomRef is required.");

        if (command.Factor <= 0)
            return Fail("Factor must be greater than zero.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        variant.AddOrUpdateConversion(command.FromUomRef, command.ToUomRef, command.Factor, command.RoundingMode, command.IsBasePath);
        await _variantRepository.CommitAsync();

        return Ok(new UpsertVariantUomConversionCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            FromUomRef = command.FromUomRef,
            ToUomRef = command.ToUomRef,
            Factor = command.Factor,
            RoundingMode = command.RoundingMode,
            IsBasePath = command.IsBasePath
        });
    }
}

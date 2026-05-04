namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.UpsertVariantComponent;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantComponent;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpsertVariantComponentCommandHandler : CommandHandler<UpsertVariantComponentCommand, UpsertVariantComponentCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public UpsertVariantComponentCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<UpsertVariantComponentCommandResult>> Handle(UpsertVariantComponentCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (command.ComponentVariantRef == Guid.Empty)
            return Fail("ComponentVariantRef is required.");

        if (command.ProductVariantBusinessKey == command.ComponentVariantRef)
            return Fail("Variant cannot reference itself as a component.");

        if (command.Quantity <= 0)
            return Fail("Quantity must be greater than zero.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        var componentExists = await _variantRepository.ExistsByBusinessKeyAsync(command.ComponentVariantRef, onlyActive: false);
        if (!componentExists)
            return Fail("Component variant was not found.");

        variant.AddOrUpdateComponent(command.ComponentVariantRef, command.Quantity);
        await _variantRepository.CommitAsync();

        return Ok(new UpsertVariantComponentCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            ComponentVariantRef = command.ComponentVariantRef,
            Quantity = command.Quantity
        });
    }
}

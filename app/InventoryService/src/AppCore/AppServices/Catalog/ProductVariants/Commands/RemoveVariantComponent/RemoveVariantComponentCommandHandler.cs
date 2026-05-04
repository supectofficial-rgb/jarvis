namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.RemoveVariantComponent;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantComponent;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RemoveVariantComponentCommandHandler : CommandHandler<RemoveVariantComponentCommand, RemoveVariantComponentCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public RemoveVariantComponentCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<RemoveVariantComponentCommandResult>> Handle(RemoveVariantComponentCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (command.ComponentVariantRef == Guid.Empty)
            return Fail("ComponentVariantRef is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        variant.RemoveComponent(command.ComponentVariantRef);
        await _variantRepository.CommitAsync();

        return Ok(new RemoveVariantComponentCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            ComponentVariantRef = command.ComponentVariantRef
        });
    }
}

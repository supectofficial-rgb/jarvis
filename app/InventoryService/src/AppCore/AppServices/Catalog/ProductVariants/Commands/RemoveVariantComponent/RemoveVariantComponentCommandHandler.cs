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
        if (command.VariantComponentBusinessKey == Guid.Empty)
            return Fail("VariantComponentBusinessKey is required.");

        var variant = await _variantRepository.GetByComponentBusinessKeyAsync(command.VariantComponentBusinessKey);
        if (variant is null)
            return Fail("Variant component was not found.");

        if (!variant.RemoveComponent(command.VariantComponentBusinessKey))
            return Fail("Variant component was not found.");

        await _variantRepository.CommitAsync();

        return Ok(new RemoveVariantComponentCommandResult
        {
            VariantComponentBusinessKey = command.VariantComponentBusinessKey
        });
    }
}

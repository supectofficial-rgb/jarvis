namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.RemoveVariantAttributeValue;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantAttributeValue;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RemoveVariantAttributeValueCommandHandler : CommandHandler<RemoveVariantAttributeValueCommand, RemoveVariantAttributeValueCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public RemoveVariantAttributeValueCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<RemoveVariantAttributeValueCommandResult>> Handle(RemoveVariantAttributeValueCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty || command.AttributeRef == Guid.Empty)
            return Fail("ProductVariantBusinessKey and AttributeRef are required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        var existed = variant.AttributeValues.Any(x => x.AttributeRef == command.AttributeRef);
        variant.RemoveAttributeValue(command.AttributeRef);
        await _variantRepository.CommitAsync();

        return Ok(new RemoveVariantAttributeValueCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            AttributeRef = command.AttributeRef,
            Removed = existed
        });
    }
}

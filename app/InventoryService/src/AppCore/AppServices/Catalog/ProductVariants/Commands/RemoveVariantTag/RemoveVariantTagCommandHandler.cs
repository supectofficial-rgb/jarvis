namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.RemoveVariantTag;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantTag;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RemoveVariantTagCommandHandler : CommandHandler<RemoveVariantTagCommand, RemoveVariantTagCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public RemoveVariantTagCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<RemoveVariantTagCommandResult>> Handle(RemoveVariantTagCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        variant.RemoveTag(command.VariantTagBusinessKey, command.TagName);
        await _variantRepository.CommitAsync();

        return Ok(new RemoveVariantTagCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            VariantTagBusinessKey = command.VariantTagBusinessKey,
            TagName = command.TagName
        });
    }
}

namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.RemoveVariantImage;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.RemoveVariantImage;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RemoveVariantImageCommandHandler : CommandHandler<RemoveVariantImageCommand, RemoveVariantImageCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;

    public RemoveVariantImageCommandHandler(IProductVariantCommandRepository variantRepository)
    {
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<RemoveVariantImageCommandResult>> Handle(RemoveVariantImageCommand command)
    {
        if (command.VariantImageBusinessKey == Guid.Empty)
            return Fail("VariantImageBusinessKey is required.");

        var deleted = await _variantRepository.DeleteVariantImageByBusinessKeyAsync(command.VariantImageBusinessKey);
        if (!deleted)
            return Fail("Variant image was not found.");

        await _variantRepository.CommitAsync();

        return Ok(new RemoveVariantImageCommandResult
        {
            VariantImageBusinessKey = command.VariantImageBusinessKey
        });
    }
}

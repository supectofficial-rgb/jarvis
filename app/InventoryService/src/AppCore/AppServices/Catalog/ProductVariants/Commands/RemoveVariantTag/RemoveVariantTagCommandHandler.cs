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
        if (command.VariantTagBusinessKey == Guid.Empty)
            return Fail("VariantTagBusinessKey is required.");

        var deleted = await _variantRepository.DeleteVariantTagByBusinessKeyAsync(command.VariantTagBusinessKey);
        if (!deleted)
            return Fail("Variant tag was not found.");

        await _variantRepository.CommitAsync();

        return Ok(new RemoveVariantTagCommandResult
        {
            VariantTagBusinessKey = command.VariantTagBusinessKey
        });
    }
}

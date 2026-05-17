namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.DeleteVariant;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.DeleteVariant;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteProductVariantCommandHandler : CommandHandler<DeleteVariantCommand, DeleteVariantCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;
    private readonly IStockDetailCommandRepository _stockDetailRepository;
    private readonly IInventoryDocumentCommandRepository _inventoryDocumentRepository;

    public DeleteProductVariantCommandHandler(
        IProductVariantCommandRepository variantRepository,
        IStockDetailCommandRepository stockDetailRepository,
        IInventoryDocumentCommandRepository inventoryDocumentRepository)
    {
        _variantRepository = variantRepository;
        _stockDetailRepository = stockDetailRepository;
        _inventoryDocumentRepository = inventoryDocumentRepository;
    }

    public override async Task<CommandResult<DeleteVariantCommandResult>> Handle(DeleteVariantCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        if (await _stockDetailRepository.ExistsByVariantRefAsync(variant.BusinessKey.Value, onlyActive: false))
            return Fail("Variant cannot be deleted because stock records exist.");

        if (await _inventoryDocumentRepository.ExistsLineByVariantRefAsync(variant.BusinessKey.Value))
            return Fail("Variant cannot be deleted because inventory documents reference it.");

        if (variant.InventoryMovementLocked)
            return Fail("Variant cannot be deleted after inventory movement has started.");

        _variantRepository.Delete(variant);
        await _variantRepository.CommitAsync();

        return Ok(new DeleteVariantCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            Deleted = true
        });
    }
}




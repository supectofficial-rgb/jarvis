namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Commands.DeleteProduct;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.DeleteProduct;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteProductCommandHandler : CommandHandler<DeleteProductCommand, DeleteProductCommandResult>
{
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;
    private readonly IStockDetailCommandRepository _stockDetailRepository;
    private readonly IInventoryDocumentCommandRepository _inventoryDocumentRepository;

    public DeleteProductCommandHandler(
        IProductCommandRepository productRepository,
        IProductVariantCommandRepository variantRepository,
        IStockDetailCommandRepository stockDetailRepository,
        IInventoryDocumentCommandRepository inventoryDocumentRepository)
    {
        _productRepository = productRepository;
        _variantRepository = variantRepository;
        _stockDetailRepository = stockDetailRepository;
        _inventoryDocumentRepository = inventoryDocumentRepository;
    }

    public override async Task<CommandResult<DeleteProductCommandResult>> Handle(DeleteProductCommand command)
    {
        if (command.ProductBusinessKey == Guid.Empty)
            return Fail("ProductBusinessKey is required.");

        var product = await _productRepository.GetByBusinessKeyAsync(command.ProductBusinessKey);
        if (product is null)
            return Fail("Product was not found.");

        var variants = await _variantRepository.GetByProductRefAsync(command.ProductBusinessKey);
        foreach (var variant in variants)
        {
            var variantKey = variant.BusinessKey.Value;

            if (await _stockDetailRepository.ExistsByVariantRefAsync(variantKey, onlyActive: false))
                return Fail($"Product cannot be deleted because variant '{variant.VariantSku}' has stock records.");

            if (await _inventoryDocumentRepository.ExistsLineByVariantRefAsync(variantKey))
                return Fail($"Product cannot be deleted because variant '{variant.VariantSku}' is used in inventory documents.");
        }

        foreach (var variant in variants)
        {
            _variantRepository.Delete(variant);
        }

        _productRepository.Delete(product);
        await _productRepository.CommitAsync();

        return Ok(new DeleteProductCommandResult
        {
            ProductBusinessKey = product.BusinessKey.Value,
            Deleted = true
        });
    }
}

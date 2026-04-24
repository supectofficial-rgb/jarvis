namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Commands.DeleteProduct;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.DeleteProduct;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteProductCommandHandler : CommandHandler<DeleteProductCommand, DeleteProductCommandResult>
{
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;

    public DeleteProductCommandHandler(IProductCommandRepository productRepository, IProductVariantCommandRepository variantRepository)
    {
        _productRepository = productRepository;
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<DeleteProductCommandResult>> Handle(DeleteProductCommand command)
    {
        if (command.ProductBusinessKey == Guid.Empty)
            return Fail("ProductBusinessKey is required.");

        var product = await _productRepository.GetByBusinessKeyAsync(command.ProductBusinessKey);
        if (product is null)
            return Fail("Product was not found.");

        var hasAnyVariants = await _variantRepository.ExistsByProductRefAsync(command.ProductBusinessKey, false);
        if (hasAnyVariants)
            return Fail("Product cannot be deleted because variants exist.");

        product.Deactivate();
        await _productRepository.CommitAsync();

        return Ok(new DeleteProductCommandResult
        {
            ProductBusinessKey = product.BusinessKey.Value,
            Deleted = true
        });
    }
}

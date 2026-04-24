namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Commands.DeactivateProduct;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.DeactivateProduct;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeactivateProductCommandHandler : CommandHandler<DeactivateProductCommand, DeactivateProductCommandResult>
{
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;

    public DeactivateProductCommandHandler(IProductCommandRepository productRepository, IProductVariantCommandRepository variantRepository)
    {
        _productRepository = productRepository;
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<DeactivateProductCommandResult>> Handle(DeactivateProductCommand command)
    {
        if (command.ProductBusinessKey == Guid.Empty)
            return Fail("ProductBusinessKey is required.");

        var product = await _productRepository.GetByBusinessKeyAsync(command.ProductBusinessKey);
        if (product is null)
            return Fail("Product was not found.");

        var hasActiveVariants = await _variantRepository.ExistsByProductRefAsync(command.ProductBusinessKey, true);
        if (hasActiveVariants)
            return Fail("Product cannot be deactivated while active variants exist.");

        product.Deactivate();
        await _productRepository.CommitAsync();

        return Ok(new DeactivateProductCommandResult
        {
            ProductBusinessKey = product.BusinessKey.Value,
            IsActive = product.IsActive
        });
    }
}

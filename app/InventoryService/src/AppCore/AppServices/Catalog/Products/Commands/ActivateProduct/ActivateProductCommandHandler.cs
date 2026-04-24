namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Commands.ActivateProduct;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.ActivateProduct;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ActivateProductCommandHandler : CommandHandler<ActivateProductCommand, ActivateProductCommandResult>
{
    private readonly IProductCommandRepository _productRepository;
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IUnitOfMeasureCommandRepository _uomRepository;

    public ActivateProductCommandHandler(
        IProductCommandRepository productRepository,
        ICategoryCommandRepository categoryRepository,
        IUnitOfMeasureCommandRepository uomRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _uomRepository = uomRepository;
    }

    public override async Task<CommandResult<ActivateProductCommandResult>> Handle(ActivateProductCommand command)
    {
        if (command.ProductBusinessKey == Guid.Empty)
            return Fail("ProductBusinessKey is required.");

        var product = await _productRepository.GetByBusinessKeyAsync(command.ProductBusinessKey);
        if (product is null)
            return Fail("Product was not found.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(product.CategoryRef);
        if (category is null || !category.IsActive)
            return Fail("Product category must be active.");

        var uom = await _uomRepository.GetByBusinessKeyAsync(product.DefaultUomRef);
        if (uom is null || !uom.IsActive)
            return Fail("Product default UOM must be active.");

        product.Activate();
        await _productRepository.CommitAsync();

        return Ok(new ActivateProductCommandResult
        {
            ProductBusinessKey = product.BusinessKey.Value,
            IsActive = product.IsActive
        });
    }
}

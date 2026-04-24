namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.ActivateVariant;

using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.ActivateVariant;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ActivateProductVariantCommandHandler : CommandHandler<ActivateVariantCommand, ActivateVariantCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;
    private readonly IProductCommandRepository _productRepository;
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IUnitOfMeasureCommandRepository _uomRepository;

    public ActivateProductVariantCommandHandler(
        IProductVariantCommandRepository variantRepository,
        IProductCommandRepository productRepository,
        ICategoryCommandRepository categoryRepository,
        IUnitOfMeasureCommandRepository uomRepository)
    {
        _variantRepository = variantRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _uomRepository = uomRepository;
    }

    public override async Task<CommandResult<ActivateVariantCommandResult>> Handle(ActivateVariantCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        var variant = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (variant is null)
            return Fail("Product variant was not found.");

        var product = await _productRepository.GetByBusinessKeyAsync(variant.ProductRef);
        if (product is null || !product.IsActive)
            return Fail("Variant product must be active.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(product.CategoryRef);
        if (category is null || !category.IsActive)
            return Fail("Variant category must be active.");

        var uom = await _uomRepository.GetByBusinessKeyAsync(variant.BaseUomRef);
        if (uom is null || !uom.IsActive)
            return Fail("Variant base UOM must be active.");

        variant.Activate();
        await _variantRepository.CommitAsync();

        return Ok(new ActivateVariantCommandResult
        {
            ProductVariantBusinessKey = variant.BusinessKey.Value,
            IsActive = variant.IsActive
        });
    }
}




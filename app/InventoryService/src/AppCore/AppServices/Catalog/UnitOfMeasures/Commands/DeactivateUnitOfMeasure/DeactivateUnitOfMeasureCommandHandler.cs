namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Commands.DeactivateUnitOfMeasure;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.DeactivateUnitOfMeasure;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeactivateUnitOfMeasureCommandHandler : CommandHandler<DeactivateUnitOfMeasureCommand, DeactivateUnitOfMeasureCommandResult>
{
    private readonly IUnitOfMeasureCommandRepository _repository;
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;

    public DeactivateUnitOfMeasureCommandHandler(
        IUnitOfMeasureCommandRepository repository,
        IProductCommandRepository productRepository,
        IProductVariantCommandRepository variantRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<DeactivateUnitOfMeasureCommandResult>> Handle(DeactivateUnitOfMeasureCommand command)
    {
        if (command.UnitOfMeasureBusinessKey == Guid.Empty)
            return Fail("UnitOfMeasureBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.UnitOfMeasureBusinessKey);
        if (aggregate is null)
            return Fail("Unit of measure was not found.");

        var usedByActiveProducts = await _productRepository.ExistsByDefaultUomRefAsync(command.UnitOfMeasureBusinessKey, onlyActive: true);
        if (usedByActiveProducts)
            return Fail("Unit of measure cannot be deactivated because active products depend on it.");

        var usedByActiveVariants = await _variantRepository.ExistsByBaseUomRefAsync(command.UnitOfMeasureBusinessKey, onlyActive: true);
        if (usedByActiveVariants)
            return Fail("Unit of measure cannot be deactivated because active variants depend on it.");

        aggregate.Deactivate();
        await _repository.CommitAsync();

        return Ok(new DeactivateUnitOfMeasureCommandResult
        {
            UnitOfMeasureBusinessKey = aggregate.BusinessKey.Value,
            IsActive = aggregate.IsActive
        });
    }
}

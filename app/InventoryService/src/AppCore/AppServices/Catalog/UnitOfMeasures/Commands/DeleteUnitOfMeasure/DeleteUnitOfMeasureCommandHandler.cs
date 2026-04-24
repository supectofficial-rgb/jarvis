namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Commands.DeleteUnitOfMeasure;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.DeleteUnitOfMeasure;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteUnitOfMeasureCommandHandler : CommandHandler<DeleteUnitOfMeasureCommand, DeleteUnitOfMeasureCommandResult>
{
    private readonly IUnitOfMeasureCommandRepository _repository;
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;

    public DeleteUnitOfMeasureCommandHandler(
        IUnitOfMeasureCommandRepository repository,
        IProductCommandRepository productRepository,
        IProductVariantCommandRepository variantRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<DeleteUnitOfMeasureCommandResult>> Handle(DeleteUnitOfMeasureCommand command)
    {
        if (command.UnitOfMeasureBusinessKey == Guid.Empty)
            return Fail("UnitOfMeasureBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.UnitOfMeasureBusinessKey);
        if (aggregate is null)
            return Fail("Unit of measure was not found.");

        var usedByProducts = await _productRepository.ExistsByDefaultUomRefAsync(command.UnitOfMeasureBusinessKey, onlyActive: false);
        if (usedByProducts)
            return Fail("Unit of measure cannot be deleted because products depend on it.");

        var usedByVariants = await _variantRepository.ExistsByBaseUomRefAsync(command.UnitOfMeasureBusinessKey, onlyActive: false);
        if (usedByVariants)
            return Fail("Unit of measure cannot be deleted because variants depend on it.");

        aggregate.Deactivate();
        await _repository.CommitAsync();

        return Ok(new DeleteUnitOfMeasureCommandResult
        {
            UnitOfMeasureBusinessKey = aggregate.BusinessKey.Value,
            Deleted = true
        });
    }
}

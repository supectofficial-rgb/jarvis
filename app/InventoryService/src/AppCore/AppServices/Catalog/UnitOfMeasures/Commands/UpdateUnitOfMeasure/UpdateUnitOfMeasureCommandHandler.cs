namespace Insurance.InventoryService.AppCore.AppServices.Catalog.UnitOfMeasures.Commands.UpdateUnitOfMeasure;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.UpdateUnitOfMeasure;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateUnitOfMeasureCommandHandler : CommandHandler<UpdateUnitOfMeasureCommand, UpdateUnitOfMeasureCommandResult>
{
    private readonly IUnitOfMeasureCommandRepository _repository;
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;

    public UpdateUnitOfMeasureCommandHandler(
        IUnitOfMeasureCommandRepository repository,
        IProductCommandRepository productRepository,
        IProductVariantCommandRepository variantRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<UpdateUnitOfMeasureCommandResult>> Handle(UpdateUnitOfMeasureCommand command)
    {
        if (command.UnitOfMeasureBusinessKey == Guid.Empty)
            return Fail("UnitOfMeasureBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        if (command.Precision < 0)
            return Fail("Precision must be greater than or equal to zero.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.UnitOfMeasureBusinessKey);
        if (aggregate is null)
            return Fail("Unit of measure was not found.");

        var normalizedCode = command.Code.Trim();
        if (!string.Equals(aggregate.Code, normalizedCode, StringComparison.OrdinalIgnoreCase)
            && await _repository.ExistsByCodeAsync(normalizedCode, command.UnitOfMeasureBusinessKey))
        {
            return Fail($"Unit of measure code '{normalizedCode}' already exists.");
        }

        if (!command.IsActive)
        {
            var usedByActiveProducts = await _productRepository.ExistsByDefaultUomRefAsync(command.UnitOfMeasureBusinessKey, onlyActive: true);
            if (usedByActiveProducts)
                return Fail("Unit of measure cannot be deactivated because active products depend on it.");

            var usedByActiveVariants = await _variantRepository.ExistsByBaseUomRefAsync(command.UnitOfMeasureBusinessKey, onlyActive: true);
            if (usedByActiveVariants)
                return Fail("Unit of measure cannot be deactivated because active variants depend on it.");
        }

        aggregate.ChangeCode(normalizedCode);
        aggregate.Rename(command.Name.Trim());
        aggregate.ChangePrecision(command.Precision);

        if (command.IsActive)
            aggregate.Activate();
        else
            aggregate.Deactivate();

        await _repository.CommitAsync();

        return Ok(new UpdateUnitOfMeasureCommandResult
        {
            UnitOfMeasureBusinessKey = aggregate.BusinessKey.Value,
            Code = aggregate.Code,
            Name = aggregate.Name,
            Precision = aggregate.Precision,
            IsActive = aggregate.IsActive
        });
    }
}

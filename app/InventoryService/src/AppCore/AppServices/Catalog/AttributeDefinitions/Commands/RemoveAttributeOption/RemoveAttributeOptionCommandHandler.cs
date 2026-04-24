namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.RemoveAttributeOption;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.RemoveAttributeOption;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RemoveAttributeOptionCommandHandler
    : CommandHandler<RemoveAttributeOptionCommand, RemoveAttributeOptionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;

    public RemoveAttributeOptionCommandHandler(
        IAttributeDefinitionCommandRepository repository,
        IProductCommandRepository productRepository,
        IProductVariantCommandRepository variantRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<RemoveAttributeOptionCommandResult>> Handle(RemoveAttributeOptionCommand command)
    {
        if (command.AttributeDefinitionBusinessKey == Guid.Empty)
            return Fail("AttributeDefinitionBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Value))
            return Fail("Option value is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.AttributeDefinitionBusinessKey);
        if (aggregate is null)
            return Fail("Attribute definition was not found.");

        var normalizedValue = command.Value.Trim();
        var existing = aggregate.Options.FirstOrDefault(x => string.Equals(x.Value, normalizedValue, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
        {
            var inUseByProducts = await _productRepository.ExistsAttributeValueByOptionRefAsync(existing.BusinessKey.Value, onlyActive: true);
            if (inUseByProducts)
                return Fail($"Option '{normalizedValue}' cannot be removed because active products are using it.");

            var inUseByVariants = await _variantRepository.ExistsAttributeValueByOptionRefAsync(existing.BusinessKey.Value, onlyActive: true);
            if (inUseByVariants)
                return Fail($"Option '{normalizedValue}' cannot be removed because active variants are using it.");
        }

        var existed = existing is not null;
        aggregate.RemoveOption(normalizedValue);
        await _repository.CommitAsync();

        return Ok(new RemoveAttributeOptionCommandResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey.Value,
            Value = normalizedValue,
            Removed = existed
        });
    }
}

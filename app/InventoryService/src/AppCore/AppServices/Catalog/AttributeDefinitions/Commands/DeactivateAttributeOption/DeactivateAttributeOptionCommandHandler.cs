namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.DeactivateAttributeOption;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeactivateAttributeOption;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeactivateAttributeOptionCommandHandler : CommandHandler<DeactivateAttributeOptionCommand, DeactivateAttributeOptionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;

    public DeactivateAttributeOptionCommandHandler(
        IAttributeDefinitionCommandRepository repository,
        IProductCommandRepository productRepository,
        IProductVariantCommandRepository variantRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<DeactivateAttributeOptionCommandResult>> Handle(DeactivateAttributeOptionCommand command)
    {
        if (command.AttributeDefinitionBusinessKey == Guid.Empty || command.OptionBusinessKey == Guid.Empty)
            return Fail("AttributeDefinitionBusinessKey and OptionBusinessKey are required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.AttributeDefinitionBusinessKey);
        if (aggregate is null)
            return Fail("Attribute definition was not found.");

        var option = aggregate.Options.FirstOrDefault(x => x.BusinessKey.Value == command.OptionBusinessKey);
        if (option is null)
            return Fail("Attribute option was not found.");

        var inUseByProducts = await _productRepository.ExistsAttributeValueByOptionRefAsync(option.BusinessKey.Value, true);
        if (inUseByProducts)
            return Fail("Option cannot be deactivated because active products are using it.");

        var inUseByVariants = await _variantRepository.ExistsAttributeValueByOptionRefAsync(option.BusinessKey.Value, true);
        if (inUseByVariants)
            return Fail("Option cannot be deactivated because active variants are using it.");

        try
        {
            aggregate.SetOptionActive(command.OptionBusinessKey, false);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();

        return Ok(new DeactivateAttributeOptionCommandResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey.Value,
            OptionBusinessKey = option.BusinessKey.Value,
            IsActive = aggregate.Options.First(x => x.BusinessKey.Value == command.OptionBusinessKey).IsActive
        });
    }
}

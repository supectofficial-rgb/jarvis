namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.DeleteAttributeOption;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeleteAttributeOption;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteAttributeOptionCommandHandler : CommandHandler<DeleteAttributeOptionCommand, DeleteAttributeOptionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;

    public DeleteAttributeOptionCommandHandler(
        IAttributeDefinitionCommandRepository repository,
        IProductCommandRepository productRepository,
        IProductVariantCommandRepository variantRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<DeleteAttributeOptionCommandResult>> Handle(DeleteAttributeOptionCommand command)
    {
        if (command.AttributeDefinitionBusinessKey == Guid.Empty || command.OptionBusinessKey == Guid.Empty)
            return Fail("AttributeDefinitionBusinessKey and OptionBusinessKey are required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.AttributeDefinitionBusinessKey);
        if (aggregate is null)
            return Fail("Attribute definition was not found.");

        var option = aggregate.Options.FirstOrDefault(x => x.BusinessKey.Value == command.OptionBusinessKey);
        if (option is null)
            return Fail("Attribute option was not found.");

        var inUseByProducts = await _productRepository.ExistsAttributeValueByOptionRefAsync(option.BusinessKey.Value, false);
        if (inUseByProducts)
            return Fail("Option cannot be deleted because products are using it.");

        var inUseByVariants = await _variantRepository.ExistsAttributeValueByOptionRefAsync(option.BusinessKey.Value, false);
        if (inUseByVariants)
            return Fail("Option cannot be deleted because variants are using it.");

        aggregate.RemoveOption(command.OptionBusinessKey);
        await _repository.CommitAsync();

        return Ok(new DeleteAttributeOptionCommandResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey.Value,
            OptionBusinessKey = command.OptionBusinessKey,
            Removed = true
        });
    }
}

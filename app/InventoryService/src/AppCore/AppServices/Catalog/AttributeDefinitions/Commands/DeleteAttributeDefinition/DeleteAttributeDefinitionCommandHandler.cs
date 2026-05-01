namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.DeleteAttributeDefinition;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.DeleteAttributeDefinition;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DeleteAttributeDefinitionCommandHandler : CommandHandler<DeleteAttributeDefinitionCommand, DeleteAttributeDefinitionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;

    public DeleteAttributeDefinitionCommandHandler(
        IAttributeDefinitionCommandRepository repository,
        ICategoryCommandRepository categoryRepository,
        IProductCommandRepository productRepository,
        IProductVariantCommandRepository variantRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
    }

    public override async Task<CommandResult<DeleteAttributeDefinitionCommandResult>> Handle(DeleteAttributeDefinitionCommand command)
    {
        if (command.AttributeDefinitionBusinessKey == Guid.Empty)
            return Fail("AttributeDefinitionBusinessKey is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.AttributeDefinitionBusinessKey);
        if (aggregate is null)
            return Fail("Attribute definition was not found.");

        bool hasCategoryUsage;
        try
        {
            hasCategoryUsage = await _categoryRepository.ExistsRuleByAttributeRefAsync(command.AttributeDefinitionBusinessKey, false, false);
        }
        catch (Exception ex)
        {
            return Fail($"Checking category usage failed: {GetExceptionMessage(ex)}");
        }

        if (hasCategoryUsage)
            return Fail("Attribute definition cannot be deleted because category rules depend on it.");

        bool hasProductUsage;
        try
        {
            hasProductUsage = await _productRepository.ExistsAttributeValueByAttributeRefAsync(command.AttributeDefinitionBusinessKey, false);
        }
        catch (Exception ex)
        {
            return Fail($"Checking product usage failed: {GetExceptionMessage(ex)}");
        }

        if (hasProductUsage)
            return Fail("Attribute definition cannot be deleted because products depend on it.");

        bool hasVariantUsage;
        try
        {
            hasVariantUsage = await _variantRepository.ExistsAttributeValueByAttributeRefAsync(command.AttributeDefinitionBusinessKey, false);
        }
        catch (Exception ex)
        {
            return Fail($"Checking variant usage failed: {GetExceptionMessage(ex)}");
        }

        if (hasVariantUsage)
            return Fail("Attribute definition cannot be deleted because variants depend on it.");

        foreach (var option in aggregate.Options.ToList())
            aggregate.RemoveOption(option.Value);

        aggregate.Deactivate();
        try
        {
            await _repository.CommitAsync();
        }
        catch (Exception ex)
        {
            return Fail($"Deleting attribute definition failed: {GetExceptionMessage(ex)}");
        }

        return Ok(new DeleteAttributeDefinitionCommandResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey.Value,
            Deleted = true
        });
    }

    private static string GetExceptionMessage(Exception exception)
    {
        var messages = new List<string>();
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (!string.IsNullOrWhiteSpace(current.Message))
                messages.Add(current.Message);
        }

        return string.Join(" | ", messages.Distinct());
    }
}

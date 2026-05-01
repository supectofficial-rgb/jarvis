namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Commands.UpdateAttributeDefinition;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands.UpdateAttributeDefinition;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateAttributeDefinitionCommandHandler
    : CommandHandler<UpdateAttributeDefinitionCommand, UpdateAttributeDefinitionCommandResult>
{
    private readonly IAttributeDefinitionCommandRepository _repository;
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IProductCommandRepository _productRepository;
    private readonly IProductVariantCommandRepository _variantRepository;

    public UpdateAttributeDefinitionCommandHandler(
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

    public override async Task<CommandResult<UpdateAttributeDefinitionCommandResult>> Handle(UpdateAttributeDefinitionCommand command)
    {
        if (command.AttributeDefinitionBusinessKey == Guid.Empty)
            return Fail("AttributeDefinitionBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        if (!Enum.TryParse<AttributeDataType>(command.DataType, true, out var dataType))
            return Fail($"Unsupported data type '{command.DataType}'.");

        if (!Enum.TryParse<AttributeScope>(command.Scope, true, out var scope))
            return Fail($"Unsupported scope '{command.Scope}'.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.AttributeDefinitionBusinessKey);
        if (aggregate is null)
            return Fail("Attribute definition was not found.");

        var normalizedCode = command.Code.Trim();
        if (!string.Equals(aggregate.Code, normalizedCode, StringComparison.OrdinalIgnoreCase)
            && await _repository.ExistsByCodeAsync(normalizedCode, command.AttributeDefinitionBusinessKey))
        {
            return Fail($"Attribute definition code '{normalizedCode}' already exists.");
        }

        var isUsedByActive = await IsAttributeUsedByActiveEntitiesAsync(command.AttributeDefinitionBusinessKey);

        if (isUsedByActive && aggregate.DataType != dataType)
            return Fail("Attribute data type cannot change while active category/product/variant data depends on it.");

        if (isUsedByActive && aggregate.Scope != scope)
            return Fail("Attribute scope cannot change while active category/product/variant data depends on it.");

        if (!command.IsActive && isUsedByActive)
            return Fail("Attribute definition cannot be deactivated while active category/product/variant data depends on it.");

        var incomingOptions = (command.Options ?? new List<UpdateAttributeOptionItem>())
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x =>
            {
                var normalizedValue = x.Value.Trim();
                var normalizedName = string.IsNullOrWhiteSpace(x.Name) ? normalizedValue : x.Name.Trim();
                return new OptionInput(normalizedName, normalizedValue, x.DisplayOrder, x.IsActive);
            })
            .ToList();

        if (incomingOptions.GroupBy(x => x.Value, StringComparer.OrdinalIgnoreCase).Any(x => x.Count() > 1))
            return Fail("Duplicate option values are not allowed.");

        if (incomingOptions.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase).Any(x => x.Count() > 1))
            return Fail("Duplicate option names are not allowed.");

        if (dataType != AttributeDataType.Option && incomingOptions.Count > 0)
            return Fail("Options are only allowed when DataType is Option.");

        var shouldReplaceOptions = dataType == AttributeDataType.Option && incomingOptions.Count > 0;

        if (shouldReplaceOptions)
        {
            var activeIncomingSet = incomingOptions
                .Where(x => x.IsActive)
                .Select(x => x.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var impactedOptions = aggregate.Options
                .Where(x => !activeIncomingSet.Contains(x.Value))
                .ToList();

            foreach (var option in impactedOptions)
            {
                var inUseByProducts = await _productRepository.ExistsAttributeValueByOptionRefAsync(option.BusinessKey.Value, onlyActive: true);
                if (inUseByProducts)
                    return Fail($"Option '{option.Value}' cannot be removed/deactivated because active products are using it.");

                var inUseByVariants = await _variantRepository.ExistsAttributeValueByOptionRefAsync(option.BusinessKey.Value, onlyActive: true);
                if (inUseByVariants)
                    return Fail($"Option '{option.Value}' cannot be removed/deactivated because active variants are using it.");
            }
        }

        aggregate.ChangeCode(normalizedCode);
        aggregate.Rename(command.Name.Trim());
        aggregate.ChangeDataType(dataType);
        aggregate.ChangeScope(scope);

        if (command.IsActive)
            aggregate.Activate();
        else
            aggregate.Deactivate();

        if (shouldReplaceOptions)
        {
            foreach (var option in incomingOptions)
            {
                if (option.IsActive)
                    aggregate.AddOption(option.Name, option.Value, option.DisplayOrder);
                else
                    aggregate.RemoveOption(option.Value);
            }

            var activeIncomingSet = incomingOptions
                .Where(x => x.IsActive)
                .Select(x => x.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var existing in aggregate.Options.ToList())
            {
                if (!activeIncomingSet.Contains(existing.Value))
                    aggregate.RemoveOption(existing.Value);
            }
        }
        else if (dataType != AttributeDataType.Option)
        {
            foreach (var existing in aggregate.Options.ToList())
                aggregate.RemoveOption(existing.Value);
        }

        await _repository.CommitAsync();

        return Ok(new UpdateAttributeDefinitionCommandResult
        {
            AttributeDefinitionBusinessKey = aggregate.BusinessKey.Value,
            Code = aggregate.Code,
            Name = aggregate.Name,
            DataType = aggregate.DataType.ToString(),
            Scope = aggregate.Scope.ToString(),
            IsActive = aggregate.IsActive
        });
    }

    private async Task<bool> IsAttributeUsedByActiveEntitiesAsync(Guid attributeDefinitionBusinessKey)
    {
        var hasCategoryUsage = await _categoryRepository.ExistsRuleByAttributeRefAsync(attributeDefinitionBusinessKey, onlyActiveCategories: true, onlyActiveRules: true);
        if (hasCategoryUsage)
            return true;

        var hasProductUsage = await _productRepository.ExistsAttributeValueByAttributeRefAsync(attributeDefinitionBusinessKey, onlyActive: true);
        if (hasProductUsage)
            return true;

        return await _variantRepository.ExistsAttributeValueByAttributeRefAsync(attributeDefinitionBusinessKey, onlyActive: true);
    }

    private sealed record OptionInput(string Name, string Value, int DisplayOrder, bool IsActive);
}

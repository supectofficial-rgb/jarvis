namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Commands.CreateProduct;

using System.Globalization;
using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.CreateProduct;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateProductCommandHandler : CommandHandler<CreateProductCommand, CreateProductCommandResult>
{
    private readonly IProductCommandRepository _productRepository;
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IUnitOfMeasureCommandRepository _uomRepository;
    private readonly IAttributeDefinitionCommandRepository _attributeRepository;

    public CreateProductCommandHandler(
        IProductCommandRepository productRepository,
        ICategoryCommandRepository categoryRepository,
        IUnitOfMeasureCommandRepository uomRepository,
        IAttributeDefinitionCommandRepository attributeRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _uomRepository = uomRepository;
        _attributeRepository = attributeRepository;
    }

    public override async Task<CommandResult<CreateProductCommandResult>> Handle(CreateProductCommand command)
    {
        if (command.CategoryRef == Guid.Empty)
            return Fail("CategoryRef is required.");

        if (command.DefaultUomRef == Guid.Empty)
            return Fail("DefaultUomRef is required.");

        if (string.IsNullOrWhiteSpace(command.BaseSku))
            return Fail("BaseSku is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        var normalizedBaseSku = command.BaseSku.Trim();
        if (await _productRepository.ExistsByBaseSkuAsync(normalizedBaseSku))
            return Fail($"Product base sku '{normalizedBaseSku}' already exists.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryRef);
        if (category is null)
            return Fail("Category was not found.");

        if (!category.IsActive)
            return Fail("Product cannot be created under inactive category.");

        var categorySchemaVersionRef = category.CurrentSchemaVersionRef;

        var uom = await _uomRepository.GetByBusinessKeyAsync(command.DefaultUomRef);
        if (uom is null)
            return Fail("Default unit of measure was not found.");

        if (!uom.IsActive)
            return Fail("Default unit of measure must be active.");

        var attributes = (command.AttributeValues ?? new List<CreateProductAttributeValueItem>())
            .Where(x => x.AttributeRef != Guid.Empty)
            .GroupBy(x => x.AttributeRef)
            .Select(x => x.Last())
            .Select(x => new ProductAttributeInput(x.AttributeRef, x.Value, x.OptionRef))
            .ToList();

        var attributeValidationError = await ValidateProductAttributesAsync(category, categorySchemaVersionRef, attributes);
        if (attributeValidationError is not null)
            return Fail(attributeValidationError);

        var aggregate = Product.Create(
            command.CategoryRef,
            categorySchemaVersionRef,
            normalizedBaseSku,
            command.Name.Trim(),
            command.DefaultUomRef,
            command.TaxCategoryRef);

        foreach (var attribute in attributes)
            aggregate.SetAttributeValue(attribute.AttributeRef, attribute.Value, attribute.OptionRef);

        await _productRepository.InsertAsync(aggregate);
        await _productRepository.CommitAsync();

        return Ok(new CreateProductCommandResult
        {
            ProductBusinessKey = aggregate.BusinessKey.Value,
            CategorySchemaVersionRef = aggregate.CategorySchemaVersionRef,
            BaseSku = aggregate.BaseSku,
            Name = aggregate.Name,
            IsActive = aggregate.IsActive
        });
    }

    private async Task<string?> ValidateProductAttributesAsync(Category category, Guid categorySchemaVersionRef, IReadOnlyCollection<ProductAttributeInput> attributes)
    {
        var productRules = category.GetAttributeRules(categorySchemaVersionRef)
            .Where(x => x.IsActive && !x.IsVariant)
            .ToList();

        if (productRules.Count > 0)
        {
            var allowedRefs = productRules.Select(x => x.AttributeRef).ToHashSet();

            var unknown = attributes
                .Where(x => !allowedRefs.Contains(x.AttributeRef))
                .Select(x => x.AttributeRef)
                .Distinct()
                .ToList();

            if (unknown.Count > 0)
                return $"Some attributes are not allowed for category '{category.Code}': {string.Join(", ", unknown)}";

            var missingRequired = productRules
                .Where(x => x.IsRequired)
                .Select(x => x.AttributeRef)
                .Where(x => attributes.All(a => a.AttributeRef != x))
                .ToList();

            if (missingRequired.Count > 0)
                return $"Required product attributes are missing: {string.Join(", ", missingRequired)}";
        }

        if (attributes.Count == 0)
            return null;

        var refs = attributes.Select(x => x.AttributeRef).Distinct().ToList();
        var definitions = await _attributeRepository.GetByBusinessKeysAsync(refs);
        var definitionMap = definitions.ToDictionary(x => x.BusinessKey.Value, x => x);

        var missingDefinitions = refs.Where(x => !definitionMap.ContainsKey(x)).ToList();
        if (missingDefinitions.Count > 0)
            return $"Some attribute definitions were not found: {string.Join(", ", missingDefinitions)}";

        foreach (var attribute in attributes)
        {
            var definition = definitionMap[attribute.AttributeRef];
            if (!definition.IsActive)
                return $"Attribute definition '{definition.Code}' is inactive.";

            if (definition.Scope == AttributeScope.Variant)
                return $"Attribute '{definition.Code}' cannot be set on product because its scope is Variant.";

            var valueError = ValidateAttributeValue(definition, attribute.Value, attribute.OptionRef);
            if (valueError is not null)
                return $"Invalid value for attribute '{definition.Code}': {valueError}";
        }

        return null;
    }

    private static string? ValidateAttributeValue(AttributeDefinition definition, string? rawValue, Guid? optionRef)
    {
        var value = string.IsNullOrWhiteSpace(rawValue) ? null : rawValue.Trim();

        if (definition.DataType == AttributeDataType.Option)
        {
            if (!optionRef.HasValue && value is null)
                return "OptionRef or Value is required for option attribute.";

            if (optionRef.HasValue)
            {
                var option = definition.Options.FirstOrDefault(x => x.BusinessKey.Value == optionRef.Value);
                if (option is null)
                    return "OptionRef does not belong to this attribute definition.";

                if (!option.IsActive)
                    return "Selected option is inactive.";

                if (value is not null && !string.Equals(value, option.Value, StringComparison.OrdinalIgnoreCase))
                    return "Value does not match selected option.";

                return null;
            }

            var byValue = definition.Options.FirstOrDefault(x => string.Equals(x.Value, value, StringComparison.OrdinalIgnoreCase));
            if (byValue is null)
                return "Value does not match any defined option.";

            if (!byValue.IsActive)
                return "Selected option value is inactive.";

            return null;
        }

        if (optionRef.HasValue)
            return "OptionRef is only valid for Option data type.";

        if (value is null)
            return null;

        return definition.DataType switch
        {
            AttributeDataType.Number => decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _)
                || decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out _)
                ? null
                : "Value must be numeric.",
            AttributeDataType.Boolean => bool.TryParse(value, out _)
                || string.Equals(value, "0", StringComparison.Ordinal)
                || string.Equals(value, "1", StringComparison.Ordinal)
                ? null
                : "Value must be boolean.",
            AttributeDataType.Date => DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out _)
                || DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                ? null
                : "Value must be a valid date.",
            _ => null
        };
    }

    private sealed record ProductAttributeInput(Guid AttributeRef, string? Value, Guid? OptionRef);
}

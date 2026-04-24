namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.CreateProductVariant;

using System.Globalization;
using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.CreateProductVariant;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateProductVariantCommandHandler : CommandHandler<CreateProductVariantCommand, CreateProductVariantCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;
    private readonly IProductCommandRepository _productRepository;
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IUnitOfMeasureCommandRepository _uomRepository;
    private readonly IAttributeDefinitionCommandRepository _attributeRepository;

    public CreateProductVariantCommandHandler(
        IProductVariantCommandRepository variantRepository,
        IProductCommandRepository productRepository,
        ICategoryCommandRepository categoryRepository,
        IUnitOfMeasureCommandRepository uomRepository,
        IAttributeDefinitionCommandRepository attributeRepository)
    {
        _variantRepository = variantRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _uomRepository = uomRepository;
        _attributeRepository = attributeRepository;
    }

    public override async Task<CommandResult<CreateProductVariantCommandResult>> Handle(CreateProductVariantCommand command)
    {
        if (command.ProductRef == Guid.Empty)
            return Fail("ProductRef is required.");

        if (command.BaseUomRef == Guid.Empty)
            return Fail("BaseUomRef is required.");

        if (string.IsNullOrWhiteSpace(command.VariantSku))
            return Fail("VariantSku is required.");

        if (!Enum.TryParse<TrackingPolicy>(command.TrackingPolicy, true, out var trackingPolicy))
            return Fail($"Unsupported tracking policy '{command.TrackingPolicy}'.");

        var product = await _productRepository.GetByBusinessKeyAsync(command.ProductRef);
        if (product is null)
            return Fail("Product was not found.");

        if (!product.IsActive)
            return Fail("Product variant cannot be created for inactive product.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(product.CategoryRef);
        if (category is null)
            return Fail("Category was not found for the selected product.");

        if (!category.IsActive)
            return Fail("Product variant cannot be created under inactive category.");

        var baseUom = await _uomRepository.GetByBusinessKeyAsync(command.BaseUomRef);
        if (baseUom is null)
            return Fail("Base unit of measure was not found.");

        if (!baseUom.IsActive)
            return Fail("Base unit of measure must be active.");

        var normalizedSku = command.VariantSku.Trim();
        if (await _variantRepository.ExistsByVariantSkuAsync(normalizedSku))
            return Fail($"Product variant sku '{normalizedSku}' already exists.");

        var normalizedBarcode = string.IsNullOrWhiteSpace(command.Barcode) ? null : command.Barcode.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedBarcode) && await _variantRepository.ExistsByBarcodeAsync(normalizedBarcode))
            return Fail($"Product variant barcode '{normalizedBarcode}' already exists.");

        var attributes = (command.AttributeValues ?? new List<CreateVariantAttributeValueItem>())
            .Where(x => x.AttributeRef != Guid.Empty)
            .GroupBy(x => x.AttributeRef)
            .Select(x => x.Last())
            .Select(x => new VariantAttributeInput(x.AttributeRef, x.Value, x.OptionRef))
            .ToList();

        var attributeValidationError = await ValidateVariantAttributesAsync(category, product.CategorySchemaVersionRef, attributes);
        if (attributeValidationError is not null)
            return Fail(attributeValidationError);

        var conversionValidationError = await ValidateConversionsAsync(command.BaseUomRef, command.UomConversions);
        if (conversionValidationError is not null)
            return Fail(conversionValidationError);

        var aggregate = ProductVariant.Create(command.ProductRef, normalizedSku, normalizedBarcode, trackingPolicy, command.BaseUomRef);

        foreach (var attribute in attributes)
            aggregate.SetAttributeValue(attribute.AttributeRef, attribute.Value, attribute.OptionRef);

        foreach (var conversion in (command.UomConversions ?? new List<CreateVariantUomConversionItem>())
                     .GroupBy(x => new { x.FromUomRef, x.ToUomRef })
                     .Select(x => x.Last()))
        {
            Enum.TryParse<UomRoundingMode>(conversion.RoundingMode, true, out var roundingMode);

            try
            {
                aggregate.AddOrUpdateConversion(conversion.FromUomRef, conversion.ToUomRef, conversion.Factor, roundingMode, conversion.IsBasePath);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        await _variantRepository.InsertAsync(aggregate);
        await _variantRepository.CommitAsync();

        return Ok(new CreateProductVariantCommandResult
        {
            ProductVariantBusinessKey = aggregate.BusinessKey.Value,
            VariantSku = aggregate.VariantSku,
            Barcode = aggregate.Barcode,
            TrackingPolicy = aggregate.TrackingPolicy.ToString(),
            IsActive = aggregate.IsActive
        });
    }

    private async Task<string?> ValidateVariantAttributesAsync(Category category, Guid categorySchemaVersionRef, IReadOnlyCollection<VariantAttributeInput> attributes)
    {
        var variantRules = category.GetAttributeRules(categorySchemaVersionRef)
            .Where(x => x.IsActive && x.IsVariant)
            .ToList();

        if (variantRules.Count > 0)
        {
            var allowedRefs = variantRules.Select(x => x.AttributeRef).ToHashSet();

            var unknown = attributes
                .Where(x => !allowedRefs.Contains(x.AttributeRef))
                .Select(x => x.AttributeRef)
                .Distinct()
                .ToList();

            if (unknown.Count > 0)
                return $"Some variant attributes are not allowed for category '{category.Code}': {string.Join(", ", unknown)}";

            var missingRequired = variantRules
                .Where(x => x.IsRequired)
                .Select(x => x.AttributeRef)
                .Where(x => attributes.All(a => a.AttributeRef != x))
                .ToList();

            if (missingRequired.Count > 0)
                return $"Required variant attributes are missing: {string.Join(", ", missingRequired)}";
        }

        if (attributes.Count == 0)
            return null;

        var refs = attributes.Select(x => x.AttributeRef).Distinct().ToList();
        var definitions = await _attributeRepository.GetByBusinessKeysAsync(refs);
        var map = definitions.ToDictionary(x => x.BusinessKey.Value, x => x);

        var missingDefinitions = refs.Where(x => !map.ContainsKey(x)).ToList();
        if (missingDefinitions.Count > 0)
            return $"Some attribute definitions were not found: {string.Join(", ", missingDefinitions)}";

        foreach (var attribute in attributes)
        {
            var definition = map[attribute.AttributeRef];
            if (!definition.IsActive)
                return $"Attribute definition '{definition.Code}' is inactive.";

            if (definition.Scope == AttributeScope.Product)
                return $"Attribute '{definition.Code}' cannot be set on variant because its scope is Product.";

            var valueError = ValidateAttributeValue(definition, attribute.Value, attribute.OptionRef);
            if (valueError is not null)
                return $"Invalid value for attribute '{definition.Code}': {valueError}";
        }

        return null;
    }

    private async Task<string?> ValidateConversionsAsync(Guid baseUomRef, IReadOnlyCollection<CreateVariantUomConversionItem>? conversions)
    {
        var items = conversions ?? new List<CreateVariantUomConversionItem>();
        if (items.Count == 0)
            return null;

        var hasInvalidRef = items.Any(x => x.FromUomRef == Guid.Empty || x.ToUomRef == Guid.Empty);
        if (hasInvalidRef)
            return "FromUomRef and ToUomRef are required for all conversions.";

        var duplicatePairs = items
            .GroupBy(x => new { x.FromUomRef, x.ToUomRef })
            .Where(x => x.Count() > 1)
            .Select(x => $"{x.Key.FromUomRef}->{x.Key.ToUomRef}")
            .ToList();

        if (duplicatePairs.Count > 0)
            return $"Duplicate conversion pairs are not allowed: {string.Join(", ", duplicatePairs)}";

        var uomRefs = items
            .SelectMany(x => new[] { x.FromUomRef, x.ToUomRef })
            .Append(baseUomRef)
            .Distinct()
            .ToList();

        var uomMap = new Dictionary<Guid, UnitOfMeasure>();
        foreach (var uomRef in uomRefs)
        {
            var uom = await _uomRepository.GetByBusinessKeyAsync(uomRef);
            if (uom is null)
                return $"Unit of measure '{uomRef}' was not found.";

            if (!uom.IsActive)
                return $"Unit of measure '{uom.Code}' must be active for conversions.";

            uomMap[uomRef] = uom;
        }

        foreach (var item in items)
        {
            if (item.FromUomRef == item.ToUomRef)
                return "Conversion from and to units cannot be the same.";

            if (item.Factor <= 0)
                return "Conversion factor must be greater than zero.";

            if (!Enum.TryParse<UomRoundingMode>(item.RoundingMode, true, out _))
                return $"Unsupported rounding mode '{item.RoundingMode}'.";

            if (item.IsBasePath && item.FromUomRef != baseUomRef && item.ToUomRef != baseUomRef)
                return "BasePath conversion must include BaseUomRef as one side of conversion.";
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

    private sealed record VariantAttributeInput(Guid AttributeRef, string? Value, Guid? OptionRef);
}

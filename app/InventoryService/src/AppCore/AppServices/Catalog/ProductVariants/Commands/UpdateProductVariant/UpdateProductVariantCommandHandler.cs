namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Commands.UpdateProductVariant;

using System.Globalization;
using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpdateProductVariant;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateProductVariantCommandHandler : CommandHandler<UpdateProductVariantCommand, UpdateProductVariantCommandResult>
{
    private readonly IProductVariantCommandRepository _variantRepository;
    private readonly IProductCommandRepository _productRepository;
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IUnitOfMeasureCommandRepository _uomRepository;
    private readonly IAttributeDefinitionCommandRepository _attributeRepository;

    public UpdateProductVariantCommandHandler(
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

    public override async Task<CommandResult<UpdateProductVariantCommandResult>> Handle(UpdateProductVariantCommand command)
    {
        if (command.ProductVariantBusinessKey == Guid.Empty)
            return Fail("ProductVariantBusinessKey is required.");

        if (command.BaseUomRef == Guid.Empty)
            return Fail("BaseUomRef is required.");

        if (string.IsNullOrWhiteSpace(command.VariantSku))
            return Fail("VariantSku is required.");

        if (!Enum.TryParse<TrackingPolicy>(command.TrackingPolicy, true, out var trackingPolicy))
            return Fail($"Unsupported tracking policy '{command.TrackingPolicy}'.");

        var aggregate = await _variantRepository.GetByBusinessKeyAsync(command.ProductVariantBusinessKey);
        if (aggregate is null)
            return Fail("Product variant was not found.");

        var product = await _productRepository.GetByBusinessKeyAsync(aggregate.ProductRef);
        if (product is null)
            return Fail("Product was not found for this variant.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(product.CategoryRef);
        if (category is null)
            return Fail("Category was not found for this variant.");

        var baseUom = await _uomRepository.GetByBusinessKeyAsync(command.BaseUomRef);
        if (baseUom is null)
            return Fail("Base unit of measure was not found.");

        if (command.IsActive)
        {
            if (!product.IsActive)
                return Fail("Active variant cannot belong to inactive product.");

            if (!category.IsActive)
                return Fail("Active variant cannot belong to inactive category.");

            if (!baseUom.IsActive)
                return Fail("Active variant must reference an active base unit of measure.");
        }

        var normalizedSku = command.VariantSku.Trim();
        if (!string.Equals(aggregate.VariantSku, normalizedSku, StringComparison.OrdinalIgnoreCase)
            && await _variantRepository.ExistsByVariantSkuAsync(normalizedSku, command.ProductVariantBusinessKey))
        {
            return Fail($"Product variant sku '{normalizedSku}' already exists.");
        }

        var normalizedBarcode = string.IsNullOrWhiteSpace(command.Barcode) ? null : command.Barcode.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedBarcode)
            && !string.Equals(aggregate.Barcode, normalizedBarcode, StringComparison.OrdinalIgnoreCase)
            && await _variantRepository.ExistsByBarcodeAsync(normalizedBarcode, command.ProductVariantBusinessKey))
        {
            return Fail($"Product variant barcode '{normalizedBarcode}' already exists.");
        }

        var incomingAttributes = (command.AttributeValues ?? new List<UpdateVariantAttributeValueItem>())
            .Where(x => x.AttributeRef != Guid.Empty)
            .GroupBy(x => x.AttributeRef)
            .Select(x => x.Last())
            .Select(x => new VariantAttributeInput(x.AttributeRef, x.Value, x.OptionRef))
            .ToList();

        var attributeValidationError = await ValidateVariantAttributesAsync(category, product.CategorySchemaVersionRef, incomingAttributes);
        if (attributeValidationError is not null)
            return Fail(attributeValidationError);

        var conversionValidationError = await ValidateConversionsAsync(command.BaseUomRef, command.UomConversions);
        if (conversionValidationError is not null)
            return Fail(conversionValidationError);

        aggregate.ChangeVariantSku(normalizedSku);
        aggregate.ChangeBarcode(normalizedBarcode);

        try
        {
            aggregate.ChangeTrackingPolicy(trackingPolicy);
            aggregate.ChangeBaseUom(command.BaseUomRef);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        if (command.IsActive)
            aggregate.Activate();
        else
            aggregate.Deactivate();

        foreach (var item in incomingAttributes)
            aggregate.SetAttributeValue(item.AttributeRef, item.Value, item.OptionRef);

        var incomingAttributeSet = incomingAttributes.Select(x => x.AttributeRef).ToHashSet();
        foreach (var existing in aggregate.AttributeValues.ToList())
        {
            if (!incomingAttributeSet.Contains(existing.AttributeRef))
                aggregate.RemoveAttributeValue(existing.AttributeRef);
        }

        var incomingConversions = (command.UomConversions ?? new List<UpdateVariantUomConversionItem>())
            .GroupBy(x => new { x.FromUomRef, x.ToUomRef })
            .Select(x => x.Last())
            .ToList();

        foreach (var conversion in incomingConversions)
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

        var incomingConversionSet = incomingConversions.Select(x => (x.FromUomRef, x.ToUomRef)).ToHashSet();
        foreach (var existing in aggregate.UomConversions.ToList())
        {
            if (!incomingConversionSet.Contains((existing.FromUomRef, existing.ToUomRef)))
                aggregate.RemoveConversion(existing.FromUomRef, existing.ToUomRef);
        }

        await _variantRepository.CommitAsync();

        return Ok(new UpdateProductVariantCommandResult
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

    private async Task<string?> ValidateConversionsAsync(Guid baseUomRef, IReadOnlyCollection<UpdateVariantUomConversionItem>? conversions)
    {
        var items = conversions ?? new List<UpdateVariantUomConversionItem>();
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

        foreach (var uomRef in uomRefs)
        {
            var uom = await _uomRepository.GetByBusinessKeyAsync(uomRef);
            if (uom is null)
                return $"Unit of measure '{uomRef}' was not found.";

            if (!uom.IsActive)
                return $"Unit of measure '{uom.Code}' must be active for conversions.";
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

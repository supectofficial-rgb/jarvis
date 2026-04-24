namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Commands.SetProductAttributeValue;

using System.Globalization;
using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.SetProductAttributeValue;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class SetProductAttributeValueCommandHandler : CommandHandler<SetProductAttributeValueCommand, SetProductAttributeValueCommandResult>
{
    private readonly IProductCommandRepository _productRepository;
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IAttributeDefinitionCommandRepository _attributeRepository;

    public SetProductAttributeValueCommandHandler(
        IProductCommandRepository productRepository,
        ICategoryCommandRepository categoryRepository,
        IAttributeDefinitionCommandRepository attributeRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _attributeRepository = attributeRepository;
    }

    public override async Task<CommandResult<SetProductAttributeValueCommandResult>> Handle(SetProductAttributeValueCommand command)
    {
        if (command.ProductBusinessKey == Guid.Empty)
            return Fail("ProductBusinessKey is required.");

        if (command.AttributeRef == Guid.Empty)
            return Fail("AttributeRef is required.");

        var product = await _productRepository.GetByBusinessKeyAsync(command.ProductBusinessKey);
        if (product is null)
            return Fail("Product was not found.");

        var category = await _categoryRepository.GetByBusinessKeyAsync(product.CategoryRef);
        if (category is null)
            return Fail("Product category was not found.");

        var rules = category.GetAttributeRules(product.CategorySchemaVersionRef).Where(x => x.IsActive && !x.IsVariant).ToList();
        if (rules.Count > 0 && rules.All(x => x.AttributeRef != command.AttributeRef))
            return Fail("Attribute is not allowed for this product category.");

        var definition = await _attributeRepository.GetByBusinessKeyAsync(command.AttributeRef);
        if (definition is null)
            return Fail("Attribute definition was not found.");

        if (!definition.IsActive)
            return Fail("Attribute definition is inactive.");

        if (definition.Scope == AttributeScope.Variant)
            return Fail("Variant-scope attribute cannot be set on product.");

        var valueError = ValidateAttributeValue(definition, command.Value, command.OptionRef);
        if (valueError is not null)
            return Fail(valueError);

        product.SetAttributeValue(command.AttributeRef, command.Value, command.OptionRef);
        await _productRepository.CommitAsync();

        return Ok(new SetProductAttributeValueCommandResult
        {
            ProductBusinessKey = product.BusinessKey.Value,
            AttributeRef = command.AttributeRef,
            Value = command.Value,
            OptionRef = command.OptionRef
        });
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
}

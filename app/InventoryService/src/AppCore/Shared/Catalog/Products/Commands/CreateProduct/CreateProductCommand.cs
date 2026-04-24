namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.CreateProduct;

using OysterFx.AppCore.Shared.Commands;

public class CreateProductCommand : ICommand<CreateProductCommandResult>
{
    public Guid CategoryRef { get; set; }
    public string BaseSku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid DefaultUomRef { get; set; }
    public Guid? TaxCategoryRef { get; set; }
    public List<CreateProductAttributeValueItem> AttributeValues { get; set; } = new();
}

public class CreateProductAttributeValueItem
{
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}

namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.UpdateProduct;

using OysterFx.AppCore.Shared.Commands;

public class UpdateProductCommand : ICommand<UpdateProductCommandResult>
{
    public Guid ProductBusinessKey { get; set; }
    public Guid CategoryRef { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid DefaultUomRef { get; set; }
    public Guid? TaxCategoryRef { get; set; }
    public bool IsActive { get; set; } = true;
    public List<UpdateProductAttributeValueItem> AttributeValues { get; set; } = new();
}

public class UpdateProductAttributeValueItem
{
    public Guid AttributeRef { get; set; }
    public string? Value { get; set; }
    public Guid? OptionRef { get; set; }
}

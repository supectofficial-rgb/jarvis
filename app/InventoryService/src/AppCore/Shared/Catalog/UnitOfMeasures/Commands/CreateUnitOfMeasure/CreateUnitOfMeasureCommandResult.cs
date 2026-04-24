namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.CreateUnitOfMeasure;

public class CreateUnitOfMeasureCommandResult
{
    public Guid UnitOfMeasureBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Precision { get; set; }
    public bool IsActive { get; set; }
}

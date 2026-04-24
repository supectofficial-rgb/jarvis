namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetByBusinessKey;

public class GetPriceTypeByBusinessKeyQueryResult
{
    public Guid PriceTypeBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

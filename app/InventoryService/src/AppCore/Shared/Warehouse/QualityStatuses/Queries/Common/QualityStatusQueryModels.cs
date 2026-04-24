namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.Common;

public class QualityStatusListItem
{
    public Guid QualityStatusBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class QualityStatusLookupItem
{
    public Guid QualityStatusBusinessKey { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

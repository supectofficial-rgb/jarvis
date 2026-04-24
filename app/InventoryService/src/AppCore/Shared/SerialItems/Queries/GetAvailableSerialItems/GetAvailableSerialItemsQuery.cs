namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetAvailableSerialItems;

using OysterFx.AppCore.Shared.Queries;

public class GetAvailableSerialItemsQuery : IQuery<GetAvailableSerialItemsQueryResult>
{
    public Guid? VariantRef { get; set; }
    public Guid? WarehouseRef { get; set; }
}

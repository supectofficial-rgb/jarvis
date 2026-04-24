namespace Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Returns.Queries.Common;

public class GetReturnRequestByIdQueryResult
{
    public ReturnRequestListItem Item { get; set; } = new();
}

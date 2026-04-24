namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;

public class GetSerialItemByIdQuery : IQuery<GetSerialItemByIdQueryResult>
{
    public GetSerialItemByIdQuery(Guid serialItemId)
    {
        SerialItemId = serialItemId;
    }

    public Guid SerialItemId { get; }
}

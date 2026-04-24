namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetSerialItemByBusinessKeyQuery : IQuery<GetSerialItemByBusinessKeyQueryResult>
{
    public GetSerialItemByBusinessKeyQuery(Guid serialItemBusinessKey)
    {
        SerialItemBusinessKey = serialItemBusinessKey;
    }

    public Guid SerialItemBusinessKey { get; }
}

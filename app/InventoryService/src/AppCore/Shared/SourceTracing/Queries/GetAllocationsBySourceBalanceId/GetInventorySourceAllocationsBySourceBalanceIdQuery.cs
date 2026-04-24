namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetAllocationsBySourceBalanceId;

using OysterFx.AppCore.Shared.Queries;

public class GetInventorySourceAllocationsBySourceBalanceIdQuery : IQuery<GetInventorySourceAllocationsBySourceBalanceIdQueryResult>
{
    public GetInventorySourceAllocationsBySourceBalanceIdQuery(Guid sourceBalanceBusinessKey)
    {
        SourceBalanceBusinessKey = sourceBalanceBusinessKey;
    }

    public Guid SourceBalanceBusinessKey { get; }
}

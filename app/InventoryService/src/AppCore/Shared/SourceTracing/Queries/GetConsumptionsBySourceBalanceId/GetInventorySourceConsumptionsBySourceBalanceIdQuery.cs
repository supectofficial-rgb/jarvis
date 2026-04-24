namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetConsumptionsBySourceBalanceId;

using OysterFx.AppCore.Shared.Queries;

public class GetInventorySourceConsumptionsBySourceBalanceIdQuery : IQuery<GetInventorySourceConsumptionsBySourceBalanceIdQueryResult>
{
    public GetInventorySourceConsumptionsBySourceBalanceIdQuery(Guid sourceBalanceBusinessKey)
    {
        SourceBalanceBusinessKey = sourceBalanceBusinessKey;
    }

    public Guid SourceBalanceBusinessKey { get; }
}

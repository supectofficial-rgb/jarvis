namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetInventorySourceBalanceSummaryQuery : IQuery<GetInventorySourceBalanceSummaryQueryResult>
{
    public GetInventorySourceBalanceSummaryQuery(Guid sourceBalanceBusinessKey)
    {
        SourceBalanceBusinessKey = sourceBalanceBusinessKey;
    }

    public Guid SourceBalanceBusinessKey { get; }
}

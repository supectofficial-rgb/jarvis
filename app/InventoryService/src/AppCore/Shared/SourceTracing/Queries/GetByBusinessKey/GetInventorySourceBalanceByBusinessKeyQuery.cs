namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetInventorySourceBalanceByBusinessKeyQuery : IQuery<GetInventorySourceBalanceByBusinessKeyQueryResult>
{
    public GetInventorySourceBalanceByBusinessKeyQuery(Guid sourceBalanceBusinessKey)
    {
        SourceBalanceBusinessKey = sourceBalanceBusinessKey;
    }

    public Guid SourceBalanceBusinessKey { get; }
}

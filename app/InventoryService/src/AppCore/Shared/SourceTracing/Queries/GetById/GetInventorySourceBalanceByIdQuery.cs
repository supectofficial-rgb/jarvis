namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;

public class GetInventorySourceBalanceByIdQuery : IQuery<GetInventorySourceBalanceByIdQueryResult>
{
    public GetInventorySourceBalanceByIdQuery(Guid sourceBalanceId)
    {
        SourceBalanceId = sourceBalanceId;
    }

    public Guid SourceBalanceId { get; }
}

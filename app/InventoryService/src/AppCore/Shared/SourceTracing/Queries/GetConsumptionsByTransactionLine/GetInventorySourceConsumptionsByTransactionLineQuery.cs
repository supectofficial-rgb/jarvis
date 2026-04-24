namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetConsumptionsByTransactionLine;

using OysterFx.AppCore.Shared.Queries;

public class GetInventorySourceConsumptionsByTransactionLineQuery : IQuery<GetInventorySourceConsumptionsByTransactionLineQueryResult>
{
    public GetInventorySourceConsumptionsByTransactionLineQuery(Guid outboundTransactionLineRef)
    {
        OutboundTransactionLineRef = outboundTransactionLineRef;
    }

    public Guid OutboundTransactionLineRef { get; }
}

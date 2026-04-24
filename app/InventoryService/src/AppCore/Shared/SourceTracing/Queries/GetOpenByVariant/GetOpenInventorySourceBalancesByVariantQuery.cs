namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetOpenByVariant;

using OysterFx.AppCore.Shared.Queries;

public class GetOpenInventorySourceBalancesByVariantQuery : IQuery<GetOpenInventorySourceBalancesByVariantQueryResult>
{
    public GetOpenInventorySourceBalancesByVariantQuery(Guid variantRef)
    {
        VariantRef = variantRef;
    }

    public Guid VariantRef { get; }
}

namespace Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByVariant;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryTransactionsByVariantQuery : IQuery<GetInventoryTransactionsByVariantQueryResult>
{
    public GetInventoryTransactionsByVariantQuery(Guid variantRef)
    {
        VariantRef = variantRef;
    }

    public Guid VariantRef { get; }
}

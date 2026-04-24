namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetByVariant;

using OysterFx.AppCore.Shared.Queries;

public class GetSerialItemsByVariantQuery : IQuery<GetSerialItemsByVariantQueryResult>
{
    public GetSerialItemsByVariantQuery(Guid variantRef)
    {
        VariantRef = variantRef;
    }

    public Guid VariantRef { get; }
}

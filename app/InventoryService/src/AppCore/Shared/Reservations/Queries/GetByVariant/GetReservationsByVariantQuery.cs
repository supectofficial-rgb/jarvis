namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByVariant;

using OysterFx.AppCore.Shared.Queries;

public class GetReservationsByVariantQuery : IQuery<GetReservationsByVariantQueryResult>
{
    public GetReservationsByVariantQuery(Guid variantRef)
    {
        VariantRef = variantRef;
    }

    public Guid VariantRef { get; }
}

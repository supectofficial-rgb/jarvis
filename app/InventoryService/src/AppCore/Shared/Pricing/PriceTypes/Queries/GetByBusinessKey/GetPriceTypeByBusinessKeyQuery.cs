namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetPriceTypeByBusinessKeyQuery : IQuery<GetPriceTypeByBusinessKeyQueryResult>
{
    public GetPriceTypeByBusinessKeyQuery(Guid priceTypeBusinessKey)
    {
        PriceTypeBusinessKey = priceTypeBusinessKey;
    }

    public Guid PriceTypeBusinessKey { get; }
}

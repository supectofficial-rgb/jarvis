namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetPriceTypeLookup;

using OysterFx.AppCore.Shared.Queries;

public class GetPriceTypeLookupQuery : IQuery<GetPriceTypeLookupQueryResult>
{
    public bool IncludeInactive { get; set; }
}

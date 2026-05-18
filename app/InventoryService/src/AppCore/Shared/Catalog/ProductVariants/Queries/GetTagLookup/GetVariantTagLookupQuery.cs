namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetTagLookup;

using OysterFx.AppCore.Shared.Queries;

public sealed class GetVariantTagLookupQuery : IQuery<GetVariantTagLookupQueryResult>
{
    public string? Term { get; set; }
    public int Take { get; set; } = 50;
}

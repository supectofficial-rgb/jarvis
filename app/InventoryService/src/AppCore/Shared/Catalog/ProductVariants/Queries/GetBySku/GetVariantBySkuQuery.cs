namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetBySku;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantBySkuQuery : IQuery<GetVariantBySkuQueryResult>
{
    public GetVariantBySkuQuery(string variantSku){ VariantSku = variantSku; } public string VariantSku { get; }
}

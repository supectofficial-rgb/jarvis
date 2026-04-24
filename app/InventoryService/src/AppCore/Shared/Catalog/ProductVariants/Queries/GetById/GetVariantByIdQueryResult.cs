namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByBusinessKey;

public class GetVariantByIdQueryResult
{
    public GetProductVariantByBusinessKeyQueryResult? Item { get; set; }
}

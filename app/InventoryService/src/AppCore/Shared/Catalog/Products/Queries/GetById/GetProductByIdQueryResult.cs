namespace Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetByBusinessKey;

public class GetProductByIdQueryResult
{
    public GetProductByBusinessKeyQueryResult? Item { get; set; }
}

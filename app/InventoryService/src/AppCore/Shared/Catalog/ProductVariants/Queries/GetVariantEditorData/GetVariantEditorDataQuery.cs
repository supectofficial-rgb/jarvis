namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantEditorData;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantEditorDataQuery : IQuery<GetVariantEditorDataQueryResult>
{
    public GetVariantEditorDataQuery(Guid variantId){ VariantId = variantId; } public Guid VariantId { get; }
}

namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantCompletionStatus;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantCompletionStatusQuery : IQuery<GetVariantCompletionStatusQueryResult>
{
    public GetVariantCompletionStatusQuery(Guid variantId){ VariantId = variantId; } public Guid VariantId { get; }
}

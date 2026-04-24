namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantSummaryQuery : IQuery<GetVariantSummaryQueryResult>
{
    public GetVariantSummaryQuery(Guid variantId){ VariantId = variantId; } public Guid VariantId { get; }
}

namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.SearchVariants;

using OysterFx.AppCore.Shared.Queries;

public class SearchVariantsQuery : IQuery<SearchVariantsQueryResult>
{
    public Guid? ProductRef { get; set; } public string? VariantSku { get; set; } public string? Barcode { get; set; } public bool? IsActive { get; set; } public int Page { get; set; } = 1; public int PageSize { get; set; } = 20;
}

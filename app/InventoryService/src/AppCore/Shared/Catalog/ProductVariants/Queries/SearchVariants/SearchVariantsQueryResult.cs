namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.SearchVariants;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common; public class SearchVariantsQueryResult { public int TotalCount { get; set; } public int Page { get; set; } public int PageSize { get; set; } public List<VariantListItem> Items { get; set; } = new(); }

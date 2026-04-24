namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetMissingRequiredAttributes;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.Common; public class GetMissingRequiredVariantAttributesQueryResult { public List<MissingRequiredVariantAttributeItem> Items { get; set; } = new(); }

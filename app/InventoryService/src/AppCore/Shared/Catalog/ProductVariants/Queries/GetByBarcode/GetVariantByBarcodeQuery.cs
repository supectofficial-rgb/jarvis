namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByBarcode;

using OysterFx.AppCore.Shared.Queries;

public class GetVariantByBarcodeQuery : IQuery<GetVariantByBarcodeQueryResult>
{
    public GetVariantByBarcodeQuery(string barcode){ Barcode = barcode; } public string Barcode { get; }
}

namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetByBarcode;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByBarcode;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductVariantByBarcodeQueryHandler : QueryHandler<GetVariantByBarcodeQuery, GetVariantByBarcodeQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetProductVariantByBarcodeQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantByBarcodeQueryResult>> ExecuteAsync(GetVariantByBarcodeQuery request)
    {
        var item = await _repository.GetByBarcodeAsync(request.Barcode);
        if (item is null)
            return QueryResult<GetVariantByBarcodeQueryResult>.Fail("Variant was not found.", "NOT_FOUND");

        return QueryResult<GetVariantByBarcodeQueryResult>.Success(new GetVariantByBarcodeQueryResult { Item = item });
    }
}




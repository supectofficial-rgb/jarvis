namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetBySku;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetBySku;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductVariantBySkuQueryHandler : QueryHandler<GetVariantBySkuQuery, GetVariantBySkuQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetProductVariantBySkuQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantBySkuQueryResult>> ExecuteAsync(GetVariantBySkuQuery request)
    {
        var item = await _repository.GetBySkuAsync(request.VariantSku);
        if (item is null)
            return QueryResult<GetVariantBySkuQueryResult>.Fail("Variant was not found.", "NOT_FOUND");

        return QueryResult<GetVariantBySkuQueryResult>.Success(new GetVariantBySkuQueryResult { Item = item });
    }
}




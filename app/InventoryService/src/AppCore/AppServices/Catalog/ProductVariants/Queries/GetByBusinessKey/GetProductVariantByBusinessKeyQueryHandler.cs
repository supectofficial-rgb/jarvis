namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductVariantByBusinessKeyQueryHandler : QueryHandler<GetProductVariantByBusinessKeyQuery, GetProductVariantByBusinessKeyQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetProductVariantByBusinessKeyQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductVariantByBusinessKeyQueryResult>> ExecuteAsync(GetProductVariantByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.ProductVariantBusinessKey);
        if (item is null)
            return QueryResult<GetProductVariantByBusinessKeyQueryResult>.Fail("Product variant was not found.", "NOT_FOUND");

        return QueryResult<GetProductVariantByBusinessKeyQueryResult>.Success(item);
    }
}

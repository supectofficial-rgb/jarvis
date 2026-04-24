namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetDetailsWithVariants;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetDetailsWithVariants;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductDetailsWithVariantsQueryHandler : QueryHandler<GetProductDetailsWithVariantsQuery, GetProductDetailsWithVariantsQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductDetailsWithVariantsQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductDetailsWithVariantsQueryResult>> ExecuteAsync(GetProductDetailsWithVariantsQuery request)
    {
        var item = await _repository.GetDetailsWithVariantsAsync(request.ProductId);
        if (item is null)
            return QueryResult<GetProductDetailsWithVariantsQueryResult>.Fail("Product was not found.", "NOT_FOUND");

        return QueryResult<GetProductDetailsWithVariantsQueryResult>.Success(new GetProductDetailsWithVariantsQueryResult { Item = item });
    }
}

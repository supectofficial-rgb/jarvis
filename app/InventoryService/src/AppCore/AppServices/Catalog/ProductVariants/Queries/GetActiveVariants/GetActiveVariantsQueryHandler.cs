namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetActiveVariants;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetActiveVariants;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveVariantsQueryHandler : QueryHandler<GetActiveVariantsQuery, GetActiveVariantsQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetActiveVariantsQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveVariantsQueryResult>> ExecuteAsync(GetActiveVariantsQuery request)
    {
        var item = await _repository.GetActiveAsync();
        return QueryResult<GetActiveVariantsQueryResult>.Success(new GetActiveVariantsQueryResult { Items = item });
    }
}

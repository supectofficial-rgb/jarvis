namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetTagLookup;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetTagLookup;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public sealed class GetVariantTagLookupQueryHandler : QueryHandler<GetVariantTagLookupQuery, GetVariantTagLookupQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantTagLookupQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantTagLookupQueryResult>> ExecuteAsync(GetVariantTagLookupQuery request)
    {
        var items = await _repository.GetTagLookupAsync(request.Term, request.Take);
        return QueryResult<GetVariantTagLookupQueryResult>.Success(new GetVariantTagLookupQueryResult { Items = items });
    }
}

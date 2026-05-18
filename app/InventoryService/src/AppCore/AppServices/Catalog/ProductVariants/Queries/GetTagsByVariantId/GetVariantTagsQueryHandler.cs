namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetTagsByVariantId;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetTagsByVariantId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantTagsQueryHandler : QueryHandler<GetVariantTagsQuery, GetVariantTagsQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantTagsQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantTagsQueryResult>> ExecuteAsync(GetVariantTagsQuery request)
    {
        var items = await _repository.GetTagsByVariantIdAsync(request.VariantId);
        return QueryResult<GetVariantTagsQueryResult>.Success(new GetVariantTagsQueryResult { Items = items });
    }
}

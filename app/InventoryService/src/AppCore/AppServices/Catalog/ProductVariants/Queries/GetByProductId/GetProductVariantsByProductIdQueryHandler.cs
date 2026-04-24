namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetByProductId;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetByProductId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductVariantsByProductIdQueryHandler : QueryHandler<GetVariantsByProductIdQuery, GetVariantsByProductIdQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetProductVariantsByProductIdQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantsByProductIdQueryResult>> ExecuteAsync(GetVariantsByProductIdQuery request)
    {
        var item = await _repository.GetByProductIdAsync(request.ProductId, request.IncludeInactive);
        return QueryResult<GetVariantsByProductIdQueryResult>.Success(new GetVariantsByProductIdQueryResult { Items = item });
    }
}




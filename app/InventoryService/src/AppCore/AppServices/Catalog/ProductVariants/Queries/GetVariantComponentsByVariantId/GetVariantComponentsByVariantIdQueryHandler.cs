namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetVariantComponentsByVariantId;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantComponentsByVariantId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantComponentsByVariantIdQueryHandler : QueryHandler<GetVariantComponentsByVariantIdQuery, GetVariantComponentsByVariantIdQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantComponentsByVariantIdQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantComponentsByVariantIdQueryResult>> ExecuteAsync(GetVariantComponentsByVariantIdQuery request)
    {
        var items = await _repository.GetComponentsByVariantIdAsync(request.VariantId);
        return QueryResult<GetVariantComponentsByVariantIdQueryResult>.Success(new GetVariantComponentsByVariantIdQueryResult
        {
            Items = items
        });
    }
}

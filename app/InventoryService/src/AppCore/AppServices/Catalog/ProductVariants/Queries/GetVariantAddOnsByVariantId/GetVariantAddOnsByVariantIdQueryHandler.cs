namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetVariantAddOnsByVariantId;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantAddOnsByVariantId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantAddOnsByVariantIdQueryHandler : QueryHandler<GetVariantAddOnsByVariantIdQuery, GetVariantAddOnsByVariantIdQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantAddOnsByVariantIdQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantAddOnsByVariantIdQueryResult>> ExecuteAsync(GetVariantAddOnsByVariantIdQuery request)
    {
        var items = await _repository.GetAddOnsByVariantIdAsync(request.VariantId);
        return QueryResult<GetVariantAddOnsByVariantIdQueryResult>.Success(new GetVariantAddOnsByVariantIdQueryResult
        {
            Items = items
        });
    }
}

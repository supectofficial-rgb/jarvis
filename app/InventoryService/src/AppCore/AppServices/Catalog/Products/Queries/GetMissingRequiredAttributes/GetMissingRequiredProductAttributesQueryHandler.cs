namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetMissingRequiredAttributes;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetMissingRequiredAttributes;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetMissingRequiredProductAttributesQueryHandler : QueryHandler<GetMissingRequiredProductAttributesQuery, GetMissingRequiredProductAttributesQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetMissingRequiredProductAttributesQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetMissingRequiredProductAttributesQueryResult>> ExecuteAsync(GetMissingRequiredProductAttributesQuery request)
    {
        var items = await _repository.GetMissingRequiredAttributesAsync(request.ProductId);
        return QueryResult<GetMissingRequiredProductAttributesQueryResult>.Success(new GetMissingRequiredProductAttributesQueryResult { Items = items });
    }
}

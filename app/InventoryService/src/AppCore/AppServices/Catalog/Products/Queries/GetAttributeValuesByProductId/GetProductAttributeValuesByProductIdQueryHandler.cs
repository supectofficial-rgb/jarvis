namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetAttributeValuesByProductId;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetAttributeValuesByProductId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductAttributeValuesByProductIdQueryHandler : QueryHandler<GetProductAttributeValuesByProductIdQuery, GetProductAttributeValuesByProductIdQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductAttributeValuesByProductIdQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductAttributeValuesByProductIdQueryResult>> ExecuteAsync(GetProductAttributeValuesByProductIdQuery request)
    {
        var items = await _repository.GetAttributeValuesByProductIdAsync(request.ProductId);
        return QueryResult<GetProductAttributeValuesByProductIdQueryResult>.Success(new GetProductAttributeValuesByProductIdQueryResult { Items = items });
    }
}

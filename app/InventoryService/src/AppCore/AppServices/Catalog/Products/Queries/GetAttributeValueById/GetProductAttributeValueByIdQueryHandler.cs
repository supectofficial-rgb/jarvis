namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetAttributeValueById;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetAttributeValueById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductAttributeValueByIdQueryHandler : QueryHandler<GetProductAttributeValueByIdQuery, GetProductAttributeValueByIdQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductAttributeValueByIdQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductAttributeValueByIdQueryResult>> ExecuteAsync(GetProductAttributeValueByIdQuery request)
    {
        var item = await _repository.GetAttributeValueByIdAsync(request.ProductAttributeValueId);
        if (item is null)
            return QueryResult<GetProductAttributeValueByIdQueryResult>.Fail("Product attribute value was not found.", "NOT_FOUND");

        return QueryResult<GetProductAttributeValueByIdQueryResult>.Success(new GetProductAttributeValueByIdQueryResult { Item = item });
    }
}

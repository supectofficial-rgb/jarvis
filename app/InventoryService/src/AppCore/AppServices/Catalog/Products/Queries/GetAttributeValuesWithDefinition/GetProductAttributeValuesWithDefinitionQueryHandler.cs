namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetAttributeValuesWithDefinition;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetAttributeValuesWithDefinition;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductAttributeValuesWithDefinitionQueryHandler : QueryHandler<GetProductAttributeValuesWithDefinitionQuery, GetProductAttributeValuesWithDefinitionQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductAttributeValuesWithDefinitionQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductAttributeValuesWithDefinitionQueryResult>> ExecuteAsync(GetProductAttributeValuesWithDefinitionQuery request)
    {
        var items = await _repository.GetAttributeValuesWithDefinitionByProductIdAsync(request.ProductId);
        return QueryResult<GetProductAttributeValuesWithDefinitionQueryResult>.Success(new GetProductAttributeValuesWithDefinitionQueryResult { Items = items });
    }
}

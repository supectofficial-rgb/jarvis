namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Queries.GetList;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetList;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetAttributeDefinitionListQueryHandler
    : QueryHandler<GetAttributeDefinitionListQuery, GetAttributeDefinitionListQueryResult>
{
    private readonly IAttributeDefinitionQueryRepository _repository;

    public GetAttributeDefinitionListQueryHandler(IAttributeDefinitionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetAttributeDefinitionListQueryResult>> ExecuteAsync(GetAttributeDefinitionListQuery request)
    {
        var result = await _repository.GetListAsync(request);
        return QueryResult<GetAttributeDefinitionListQueryResult>.Success(result);
    }
}

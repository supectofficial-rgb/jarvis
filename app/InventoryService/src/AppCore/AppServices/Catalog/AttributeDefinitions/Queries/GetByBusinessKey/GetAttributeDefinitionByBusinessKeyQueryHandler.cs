namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetAttributeDefinitionByBusinessKeyQueryHandler
    : QueryHandler<GetAttributeDefinitionByBusinessKeyQuery, GetAttributeDefinitionByBusinessKeyQueryResult>
{
    private readonly IAttributeDefinitionQueryRepository _repository;

    public GetAttributeDefinitionByBusinessKeyQueryHandler(IAttributeDefinitionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetAttributeDefinitionByBusinessKeyQueryResult>> ExecuteAsync(GetAttributeDefinitionByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.AttributeDefinitionBusinessKey);
        if (item is null)
            return QueryResult<GetAttributeDefinitionByBusinessKeyQueryResult>.Fail("Attribute definition was not found.", "NOT_FOUND");

        return QueryResult<GetAttributeDefinitionByBusinessKeyQueryResult>.Success(item);
    }
}

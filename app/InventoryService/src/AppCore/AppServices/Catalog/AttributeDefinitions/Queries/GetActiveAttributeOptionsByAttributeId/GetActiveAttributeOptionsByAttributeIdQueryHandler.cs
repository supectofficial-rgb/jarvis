namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Queries.GetActiveAttributeOptionsByAttributeId;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetActiveAttributeOptionsByAttributeId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveAttributeOptionsByAttributeIdQueryHandler : QueryHandler<GetActiveAttributeOptionsByAttributeIdQuery, GetActiveAttributeOptionsByAttributeIdQueryResult>
{
    private readonly IAttributeDefinitionQueryRepository _repository;

    public GetActiveAttributeOptionsByAttributeIdQueryHandler(IAttributeDefinitionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveAttributeOptionsByAttributeIdQueryResult>> ExecuteAsync(GetActiveAttributeOptionsByAttributeIdQuery request)
    {
        var items = await _repository.GetOptionsByAttributeIdAsync(request.AttributeDefinitionId, includeInactive: false);
        return QueryResult<GetActiveAttributeOptionsByAttributeIdQueryResult>.Success(new GetActiveAttributeOptionsByAttributeIdQueryResult { Items = items });
    }
}

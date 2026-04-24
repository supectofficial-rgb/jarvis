namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Queries.GetAttributeOptionsByAttributeId;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetAttributeOptionsByAttributeId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetAttributeOptionsByAttributeIdQueryHandler : QueryHandler<GetAttributeOptionsByAttributeIdQuery, GetAttributeOptionsByAttributeIdQueryResult>
{
    private readonly IAttributeDefinitionQueryRepository _repository;

    public GetAttributeOptionsByAttributeIdQueryHandler(IAttributeDefinitionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetAttributeOptionsByAttributeIdQueryResult>> ExecuteAsync(GetAttributeOptionsByAttributeIdQuery request)
    {
        var items = await _repository.GetOptionsByAttributeIdAsync(request.AttributeDefinitionId, includeInactive: true);
        return QueryResult<GetAttributeOptionsByAttributeIdQueryResult>.Success(new GetAttributeOptionsByAttributeIdQueryResult { Items = items });
    }
}

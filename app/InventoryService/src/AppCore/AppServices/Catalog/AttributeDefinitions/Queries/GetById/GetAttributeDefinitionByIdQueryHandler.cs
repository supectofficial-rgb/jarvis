namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetAttributeDefinitionByIdQueryHandler : QueryHandler<GetAttributeDefinitionByIdQuery, GetAttributeDefinitionByIdQueryResult>
{
    private readonly IAttributeDefinitionQueryRepository _repository;

    public GetAttributeDefinitionByIdQueryHandler(IAttributeDefinitionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetAttributeDefinitionByIdQueryResult>> ExecuteAsync(GetAttributeDefinitionByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.AttributeDefinitionId);
        if (item is null)
            return QueryResult<GetAttributeDefinitionByIdQueryResult>.Fail("Attribute definition was not found.", "NOT_FOUND");

        return QueryResult<GetAttributeDefinitionByIdQueryResult>.Success(new GetAttributeDefinitionByIdQueryResult { Item = item });
    }
}

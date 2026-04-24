namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Queries.GetAttributeOptionById;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetAttributeOptionById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetAttributeOptionByIdQueryHandler : QueryHandler<GetAttributeOptionByIdQuery, GetAttributeOptionByIdQueryResult>
{
    private readonly IAttributeDefinitionQueryRepository _repository;

    public GetAttributeOptionByIdQueryHandler(IAttributeDefinitionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetAttributeOptionByIdQueryResult>> ExecuteAsync(GetAttributeOptionByIdQuery request)
    {
        var item = await _repository.GetOptionByIdAsync(request.OptionId);
        if (item is null)
            return QueryResult<GetAttributeOptionByIdQueryResult>.Fail("Attribute option was not found.", "NOT_FOUND");

        return QueryResult<GetAttributeOptionByIdQueryResult>.Success(new GetAttributeOptionByIdQueryResult { Item = item });
    }
}

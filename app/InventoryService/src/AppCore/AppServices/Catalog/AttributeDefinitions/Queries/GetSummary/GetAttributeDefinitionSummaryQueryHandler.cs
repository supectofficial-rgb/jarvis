namespace Insurance.InventoryService.AppCore.AppServices.Catalog.AttributeDefinitions.Queries.GetSummary;

using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetAttributeDefinitionSummaryQueryHandler : QueryHandler<GetAttributeDefinitionSummaryQuery, GetAttributeDefinitionSummaryQueryResult>
{
    private readonly IAttributeDefinitionQueryRepository _repository;

    public GetAttributeDefinitionSummaryQueryHandler(IAttributeDefinitionQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetAttributeDefinitionSummaryQueryResult>> ExecuteAsync(GetAttributeDefinitionSummaryQuery request)
    {
        var item = await _repository.GetSummaryAsync(request.AttributeDefinitionId);
        if (item is null)
            return QueryResult<GetAttributeDefinitionSummaryQueryResult>.Fail("Attribute definition was not found.", "NOT_FOUND");

        return QueryResult<GetAttributeDefinitionSummaryQueryResult>.Success(new GetAttributeDefinitionSummaryQueryResult { Item = item });
    }
}

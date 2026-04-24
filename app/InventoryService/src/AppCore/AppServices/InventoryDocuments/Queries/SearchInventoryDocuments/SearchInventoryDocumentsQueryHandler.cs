namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Queries.SearchInventoryDocuments;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.SearchInventoryDocuments;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchInventoryDocumentsQueryHandler
    : QueryHandler<SearchInventoryDocumentsQuery, SearchInventoryDocumentsQueryResult>
{
    private readonly IInventoryDocumentQueryRepository _repository;

    public SearchInventoryDocumentsQueryHandler(IInventoryDocumentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchInventoryDocumentsQueryResult>> ExecuteAsync(SearchInventoryDocumentsQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchInventoryDocumentsQueryResult>.Success(result);
    }
}

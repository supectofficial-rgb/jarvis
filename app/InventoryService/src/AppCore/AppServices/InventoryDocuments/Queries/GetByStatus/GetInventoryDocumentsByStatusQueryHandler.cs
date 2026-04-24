namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Queries.GetByStatus;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByStatus;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryDocumentsByStatusQueryHandler
    : QueryHandler<GetInventoryDocumentsByStatusQuery, GetInventoryDocumentsByStatusQueryResult>
{
    private readonly IInventoryDocumentQueryRepository _repository;

    public GetInventoryDocumentsByStatusQueryHandler(IInventoryDocumentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryDocumentsByStatusQueryResult>> ExecuteAsync(GetInventoryDocumentsByStatusQuery request)
    {
        var items = await _repository.GetByStatusAsync(request.Status);
        return QueryResult<GetInventoryDocumentsByStatusQueryResult>.Success(new GetInventoryDocumentsByStatusQueryResult { Items = items });
    }
}

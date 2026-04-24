namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Queries.GetByType;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByType;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryDocumentsByTypeQueryHandler
    : QueryHandler<GetInventoryDocumentsByTypeQuery, GetInventoryDocumentsByTypeQueryResult>
{
    private readonly IInventoryDocumentQueryRepository _repository;

    public GetInventoryDocumentsByTypeQueryHandler(IInventoryDocumentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryDocumentsByTypeQueryResult>> ExecuteAsync(GetInventoryDocumentsByTypeQuery request)
    {
        var items = await _repository.GetByTypeAsync(request.DocumentType);
        return QueryResult<GetInventoryDocumentsByTypeQueryResult>.Success(new GetInventoryDocumentsByTypeQueryResult { Items = items });
    }
}

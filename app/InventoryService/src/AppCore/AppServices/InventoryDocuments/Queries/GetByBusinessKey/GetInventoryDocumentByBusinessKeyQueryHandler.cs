namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryDocumentByBusinessKeyQueryHandler
    : QueryHandler<GetInventoryDocumentByBusinessKeyQuery, GetInventoryDocumentByBusinessKeyQueryResult>
{
    private readonly IInventoryDocumentQueryRepository _repository;

    public GetInventoryDocumentByBusinessKeyQueryHandler(IInventoryDocumentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryDocumentByBusinessKeyQueryResult>> ExecuteAsync(GetInventoryDocumentByBusinessKeyQuery request)
    {
        var document = await _repository.GetByBusinessKeyAsync(request.DocumentBusinessKey);
        if (document is null)
            return QueryResult<GetInventoryDocumentByBusinessKeyQueryResult>.Fail("Inventory document was not found.", "NOT_FOUND");

        return QueryResult<GetInventoryDocumentByBusinessKeyQueryResult>.Success(document);
    }
}

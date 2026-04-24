namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Queries.GetByNo;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByNo;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryDocumentByNoQueryHandler
    : QueryHandler<GetInventoryDocumentByNoQuery, GetInventoryDocumentByNoQueryResult>
{
    private readonly IInventoryDocumentQueryRepository _repository;

    public GetInventoryDocumentByNoQueryHandler(IInventoryDocumentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryDocumentByNoQueryResult>> ExecuteAsync(GetInventoryDocumentByNoQuery request)
    {
        var item = await _repository.GetByNoAsync(request.DocumentNo);
        if (item is null)
            return QueryResult<GetInventoryDocumentByNoQueryResult>.Fail("Inventory document was not found.", "NOT_FOUND");

        return QueryResult<GetInventoryDocumentByNoQueryResult>.Success(new GetInventoryDocumentByNoQueryResult { Item = item });
    }
}

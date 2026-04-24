namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryDocumentByIdQueryHandler
    : QueryHandler<GetInventoryDocumentByIdQuery, GetInventoryDocumentByIdQueryResult>
{
    private readonly IInventoryDocumentQueryRepository _repository;

    public GetInventoryDocumentByIdQueryHandler(IInventoryDocumentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryDocumentByIdQueryResult>> ExecuteAsync(GetInventoryDocumentByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.DocumentId);
        if (item is null)
            return QueryResult<GetInventoryDocumentByIdQueryResult>.Fail("Inventory document was not found.", "NOT_FOUND");

        return QueryResult<GetInventoryDocumentByIdQueryResult>.Success(new GetInventoryDocumentByIdQueryResult { Item = item });
    }
}

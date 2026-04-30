namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Queries.GetLinesByDocument;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetLinesByDocument;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryDocumentLinesByDocumentQueryHandler
    : QueryHandler<GetInventoryDocumentLinesByDocumentQuery, GetInventoryDocumentLinesByDocumentQueryResult>
{
    private readonly IInventoryDocumentQueryRepository _repository;

    public GetInventoryDocumentLinesByDocumentQueryHandler(IInventoryDocumentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryDocumentLinesByDocumentQueryResult>> ExecuteAsync(GetInventoryDocumentLinesByDocumentQuery request)
    {
        var result = await _repository.GetLinesByDocumentAsync(request.DocumentBusinessKey);
        return QueryResult<GetInventoryDocumentLinesByDocumentQueryResult>.Success(result);
    }
}

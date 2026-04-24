namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetProductCompletionStatus;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetProductCompletionStatus;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductCompletionStatusQueryHandler : QueryHandler<GetProductCompletionStatusQuery, GetProductCompletionStatusQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductCompletionStatusQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductCompletionStatusQueryResult>> ExecuteAsync(GetProductCompletionStatusQuery request)
    {
        var item = await _repository.GetCompletionStatusAsync(request.ProductId);
        if (item is null)
            return QueryResult<GetProductCompletionStatusQueryResult>.Fail("Product was not found.", "NOT_FOUND");

        return QueryResult<GetProductCompletionStatusQueryResult>.Success(new GetProductCompletionStatusQueryResult { Item = item });
    }
}

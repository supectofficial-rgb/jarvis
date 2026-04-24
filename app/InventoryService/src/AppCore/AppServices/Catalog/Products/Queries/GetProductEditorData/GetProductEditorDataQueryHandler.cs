namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Queries.GetProductEditorData;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Queries.GetProductEditorData;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductEditorDataQueryHandler : QueryHandler<GetProductEditorDataQuery, GetProductEditorDataQueryResult>
{
    private readonly IProductQueryRepository _repository;

    public GetProductEditorDataQueryHandler(IProductQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetProductEditorDataQueryResult>> ExecuteAsync(GetProductEditorDataQuery request)
    {
        var item = await _repository.GetEditorDataAsync(request.ProductId);
        if (item is null)
            return QueryResult<GetProductEditorDataQueryResult>.Fail("Product was not found.", "NOT_FOUND");

        return QueryResult<GetProductEditorDataQueryResult>.Success(new GetProductEditorDataQueryResult { Item = item });
    }
}

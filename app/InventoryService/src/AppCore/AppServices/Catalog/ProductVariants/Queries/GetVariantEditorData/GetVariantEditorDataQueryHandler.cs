namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetVariantEditorData;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantEditorData;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantEditorDataQueryHandler : QueryHandler<GetVariantEditorDataQuery, GetVariantEditorDataQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantEditorDataQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantEditorDataQueryResult>> ExecuteAsync(GetVariantEditorDataQuery request)
    {
        var item = await _repository.GetEditorDataAsync(request.VariantId);
        if (item is null)
            return QueryResult<GetVariantEditorDataQueryResult>.Fail("Variant was not found.", "NOT_FOUND");

        return QueryResult<GetVariantEditorDataQueryResult>.Success(new GetVariantEditorDataQueryResult { Item = item });
    }
}

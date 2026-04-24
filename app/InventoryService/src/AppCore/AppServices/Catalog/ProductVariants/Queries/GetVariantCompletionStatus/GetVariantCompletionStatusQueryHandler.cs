namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetVariantCompletionStatus;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantCompletionStatus;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantCompletionStatusQueryHandler : QueryHandler<GetVariantCompletionStatusQuery, GetVariantCompletionStatusQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantCompletionStatusQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantCompletionStatusQueryResult>> ExecuteAsync(GetVariantCompletionStatusQuery request)
    {
        var item = await _repository.GetCompletionStatusAsync(request.VariantId);
        if (item is null)
            return QueryResult<GetVariantCompletionStatusQueryResult>.Fail("Variant was not found.", "NOT_FOUND");

        return QueryResult<GetVariantCompletionStatusQueryResult>.Success(new GetVariantCompletionStatusQueryResult { Item = item });
    }
}

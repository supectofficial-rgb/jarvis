namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetSummary;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantSummaryQueryHandler : QueryHandler<GetVariantSummaryQuery, GetVariantSummaryQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantSummaryQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantSummaryQueryResult>> ExecuteAsync(GetVariantSummaryQuery request)
    {
        var item = await _repository.GetSummaryAsync(request.VariantId);
        if (item is null)
            return QueryResult<GetVariantSummaryQueryResult>.Fail("Variant was not found.", "NOT_FOUND");

        return QueryResult<GetVariantSummaryQueryResult>.Success(new GetVariantSummaryQueryResult { Item = item });
    }
}

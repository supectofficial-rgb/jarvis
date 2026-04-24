namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetFullDetails;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetFullDetails;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductVariantFullDetailsQueryHandler : QueryHandler<GetVariantFullDetailsQuery, GetVariantFullDetailsQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetProductVariantFullDetailsQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantFullDetailsQueryResult>> ExecuteAsync(GetVariantFullDetailsQuery request)
    {
        var item = await _repository.GetFullDetailsAsync(request.VariantId);
        if (item is null)
            return QueryResult<GetVariantFullDetailsQueryResult>.Fail("Variant was not found.", "NOT_FOUND");

        return QueryResult<GetVariantFullDetailsQueryResult>.Success(new GetVariantFullDetailsQueryResult { Item = item });
    }
}




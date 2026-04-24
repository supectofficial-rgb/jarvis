namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetDetailsWithProductContext;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetDetailsWithProductContext;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantDetailsWithProductContextQueryHandler : QueryHandler<GetVariantDetailsWithProductContextQuery, GetVariantDetailsWithProductContextQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantDetailsWithProductContextQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantDetailsWithProductContextQueryResult>> ExecuteAsync(GetVariantDetailsWithProductContextQuery request)
    {
        var item = await _repository.GetDetailsWithProductContextAsync(request.VariantId);
        if (item is null)
            return QueryResult<GetVariantDetailsWithProductContextQueryResult>.Fail("Variant was not found.", "NOT_FOUND");

        return QueryResult<GetVariantDetailsWithProductContextQueryResult>.Success(new GetVariantDetailsWithProductContextQueryResult { Item = item });
    }
}

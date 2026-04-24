namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetDetailsWithAttributes;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetDetailsWithAttributes;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantDetailsWithAttributesQueryHandler : QueryHandler<GetVariantDetailsWithAttributesQuery, GetVariantDetailsWithAttributesQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantDetailsWithAttributesQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantDetailsWithAttributesQueryResult>> ExecuteAsync(GetVariantDetailsWithAttributesQuery request)
    {
        var item = await _repository.GetDetailsWithAttributesAsync(request.VariantId);
        if (item is null)
            return QueryResult<GetVariantDetailsWithAttributesQueryResult>.Fail("Variant was not found.", "NOT_FOUND");

        return QueryResult<GetVariantDetailsWithAttributesQueryResult>.Success(new GetVariantDetailsWithAttributesQueryResult { Item = item });
    }
}

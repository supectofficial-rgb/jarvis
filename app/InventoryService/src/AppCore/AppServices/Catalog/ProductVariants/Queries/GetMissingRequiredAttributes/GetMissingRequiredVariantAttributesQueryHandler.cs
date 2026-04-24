namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetMissingRequiredAttributes;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetMissingRequiredAttributes;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetMissingRequiredVariantAttributesQueryHandler : QueryHandler<GetMissingRequiredVariantAttributesQuery, GetMissingRequiredVariantAttributesQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetMissingRequiredVariantAttributesQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetMissingRequiredVariantAttributesQueryResult>> ExecuteAsync(GetMissingRequiredVariantAttributesQuery request)
    {
        var item = await _repository.GetMissingRequiredAttributesAsync(request.VariantId);
        return QueryResult<GetMissingRequiredVariantAttributesQueryResult>.Success(new GetMissingRequiredVariantAttributesQueryResult { Items = item });
    }
}

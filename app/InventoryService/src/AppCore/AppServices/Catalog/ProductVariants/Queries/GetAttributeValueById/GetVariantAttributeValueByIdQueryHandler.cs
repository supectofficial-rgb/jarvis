namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetAttributeValueById;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetAttributeValueById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantAttributeValueByIdQueryHandler : QueryHandler<GetVariantAttributeValueByIdQuery, GetVariantAttributeValueByIdQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantAttributeValueByIdQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantAttributeValueByIdQueryResult>> ExecuteAsync(GetVariantAttributeValueByIdQuery request)
    {
        var item = await _repository.GetAttributeValueByIdAsync(request.VariantAttributeValueId);
        if (item is null)
            return QueryResult<GetVariantAttributeValueByIdQueryResult>.Fail("Variant attribute value was not found.", "NOT_FOUND");

        return QueryResult<GetVariantAttributeValueByIdQueryResult>.Success(new GetVariantAttributeValueByIdQueryResult { Item = item });
    }
}

namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetAttributeValuesByVariantId;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetAttributeValuesByVariantId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantAttributeValuesByVariantIdQueryHandler : QueryHandler<GetVariantAttributeValuesByVariantIdQuery, GetVariantAttributeValuesByVariantIdQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantAttributeValuesByVariantIdQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantAttributeValuesByVariantIdQueryResult>> ExecuteAsync(GetVariantAttributeValuesByVariantIdQuery request)
    {
        var item = await _repository.GetAttributeValuesByVariantIdAsync(request.VariantId);
        return QueryResult<GetVariantAttributeValuesByVariantIdQueryResult>.Success(new GetVariantAttributeValuesByVariantIdQueryResult { Items = item });
    }
}

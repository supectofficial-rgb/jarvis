namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetVariantUomConversionsByVariantId;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantUomConversionsByVariantId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantUomConversionsByVariantIdQueryHandler : QueryHandler<GetVariantUomConversionsByVariantIdQuery, GetVariantUomConversionsByVariantIdQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantUomConversionsByVariantIdQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantUomConversionsByVariantIdQueryResult>> ExecuteAsync(GetVariantUomConversionsByVariantIdQuery request)
    {
        var items = await _repository.GetUomConversionsByVariantIdAsync(request.VariantId);
        return QueryResult<GetVariantUomConversionsByVariantIdQueryResult>.Success(new GetVariantUomConversionsByVariantIdQueryResult
        {
            Items = items
        });
    }
}

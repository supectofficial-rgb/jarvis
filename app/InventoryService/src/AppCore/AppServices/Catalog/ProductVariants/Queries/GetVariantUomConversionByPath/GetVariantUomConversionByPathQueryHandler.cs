namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetVariantUomConversionByPath;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantUomConversionByPath;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantUomConversionByPathQueryHandler : QueryHandler<GetVariantUomConversionByPathQuery, GetVariantUomConversionByPathQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantUomConversionByPathQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantUomConversionByPathQueryResult>> ExecuteAsync(GetVariantUomConversionByPathQuery request)
    {
        var item = await _repository.GetUomConversionByPathAsync(request.VariantId, request.FromUomRef, request.ToUomRef);
        if (item is null)
            return QueryResult<GetVariantUomConversionByPathQueryResult>.Fail("Variant UOM conversion was not found.", "NOT_FOUND");

        return QueryResult<GetVariantUomConversionByPathQueryResult>.Success(new GetVariantUomConversionByPathQueryResult
        {
            Item = item
        });
    }
}

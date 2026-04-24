namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetVariantCatalogForm;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetVariantCatalogForm;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantCatalogFormQueryHandler : QueryHandler<GetVariantCatalogFormQuery, GetVariantCatalogFormQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantCatalogFormQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantCatalogFormQueryResult>> ExecuteAsync(GetVariantCatalogFormQuery request)
    {
        var item = await _repository.GetCatalogFormAsync(request.VariantId);
        if (item is null)
            return QueryResult<GetVariantCatalogFormQueryResult>.Fail("Variant was not found.", "NOT_FOUND");

        return QueryResult<GetVariantCatalogFormQueryResult>.Success(new GetVariantCatalogFormQueryResult { Item = item });
    }
}

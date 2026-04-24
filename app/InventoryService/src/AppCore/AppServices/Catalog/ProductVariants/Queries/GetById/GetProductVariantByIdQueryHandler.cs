namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetProductVariantByIdQueryHandler : QueryHandler<GetVariantByIdQuery, GetVariantByIdQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetProductVariantByIdQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantByIdQueryResult>> ExecuteAsync(GetVariantByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.VariantId);
        if (item is null)
            return QueryResult<GetVariantByIdQueryResult>.Fail("Variant was not found.", "NOT_FOUND");

        return QueryResult<GetVariantByIdQueryResult>.Success(new GetVariantByIdQueryResult { Item = item });
    }
}




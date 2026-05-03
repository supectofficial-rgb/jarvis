namespace Insurance.InventoryService.AppCore.AppServices.Catalog.VariantNameFormulas.Queries.GetByCategoryId;

using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries.GetByCategoryId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategoryVariantNameFormulasByCategoryIdQueryHandler
    : QueryHandler<GetCategoryVariantNameFormulasByCategoryIdQuery, GetCategoryVariantNameFormulasByCategoryIdQueryResult>
{
    private readonly ICategoryVariantNameFormulaQueryRepository _repository;

    public GetCategoryVariantNameFormulasByCategoryIdQueryHandler(ICategoryVariantNameFormulaQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategoryVariantNameFormulasByCategoryIdQueryResult>> ExecuteAsync(
        GetCategoryVariantNameFormulasByCategoryIdQuery request)
    {
        return QueryResult<GetCategoryVariantNameFormulasByCategoryIdQueryResult>.Success(
            new GetCategoryVariantNameFormulasByCategoryIdQueryResult
            {
                CategoryRef = request.CategoryRef,
                Items = await _repository.GetByCategoryIdAsync(request.CategoryRef, request.IncludeInactive)
            });
    }
}

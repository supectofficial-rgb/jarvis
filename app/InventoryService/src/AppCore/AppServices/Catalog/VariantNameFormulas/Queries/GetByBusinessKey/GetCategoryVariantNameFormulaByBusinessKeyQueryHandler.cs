namespace Insurance.InventoryService.AppCore.AppServices.Catalog.VariantNameFormulas.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.VariantNameFormulas.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetCategoryVariantNameFormulaByBusinessKeyQueryHandler
    : QueryHandler<GetCategoryVariantNameFormulaByBusinessKeyQuery, GetCategoryVariantNameFormulaByBusinessKeyQueryResult>
{
    private readonly ICategoryVariantNameFormulaQueryRepository _repository;

    public GetCategoryVariantNameFormulaByBusinessKeyQueryHandler(ICategoryVariantNameFormulaQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetCategoryVariantNameFormulaByBusinessKeyQueryResult>> ExecuteAsync(
        GetCategoryVariantNameFormulaByBusinessKeyQuery request)
    {
        return QueryResult<GetCategoryVariantNameFormulaByBusinessKeyQueryResult>.Success(
            new GetCategoryVariantNameFormulaByBusinessKeyQueryResult
            {
                Item = await _repository.GetByBusinessKeyAsync(request.FormulaBusinessKey)
            });
    }
}

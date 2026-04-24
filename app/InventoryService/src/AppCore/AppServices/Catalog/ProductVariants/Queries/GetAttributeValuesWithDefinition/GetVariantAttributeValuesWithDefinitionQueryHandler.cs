namespace Insurance.InventoryService.AppCore.AppServices.Catalog.ProductVariants.Queries.GetAttributeValuesWithDefinition;

using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Queries.GetAttributeValuesWithDefinition;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetVariantAttributeValuesWithDefinitionQueryHandler : QueryHandler<GetVariantAttributeValuesWithDefinitionQuery, GetVariantAttributeValuesWithDefinitionQueryResult>
{
    private readonly IProductVariantQueryRepository _repository;

    public GetVariantAttributeValuesWithDefinitionQueryHandler(IProductVariantQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetVariantAttributeValuesWithDefinitionQueryResult>> ExecuteAsync(GetVariantAttributeValuesWithDefinitionQuery request)
    {
        var item = await _repository.GetAttributeValuesWithDefinitionByVariantIdAsync(request.VariantId);
        return QueryResult<GetVariantAttributeValuesWithDefinitionQueryResult>.Success(new GetVariantAttributeValuesWithDefinitionQueryResult { Items = item });
    }
}

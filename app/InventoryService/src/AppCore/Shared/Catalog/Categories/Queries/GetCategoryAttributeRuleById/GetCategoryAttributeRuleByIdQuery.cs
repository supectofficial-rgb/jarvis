namespace Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Queries.GetCategoryAttributeRuleById;

using OysterFx.AppCore.Shared.Queries;

public class GetCategoryAttributeRuleByIdQuery : IQuery<GetCategoryAttributeRuleByIdQueryResult>
{
    public GetCategoryAttributeRuleByIdQuery(Guid categoryAttributeRuleId)
    {
        CategoryAttributeRuleId = categoryAttributeRuleId;
    }

    public Guid CategoryAttributeRuleId { get; }
}

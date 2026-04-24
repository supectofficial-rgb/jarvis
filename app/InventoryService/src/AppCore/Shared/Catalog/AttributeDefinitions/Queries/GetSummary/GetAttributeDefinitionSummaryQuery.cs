namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetAttributeDefinitionSummaryQuery : IQuery<GetAttributeDefinitionSummaryQueryResult>
{
    public GetAttributeDefinitionSummaryQuery(Guid attributeDefinitionId)
    {
        AttributeDefinitionId = attributeDefinitionId;
    }

    public Guid AttributeDefinitionId { get; }
}

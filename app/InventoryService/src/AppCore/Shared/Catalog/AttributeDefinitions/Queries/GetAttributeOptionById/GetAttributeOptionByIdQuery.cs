namespace Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Queries.GetAttributeOptionById;

using OysterFx.AppCore.Shared.Queries;

public class GetAttributeOptionByIdQuery : IQuery<GetAttributeOptionByIdQueryResult>
{
    public GetAttributeOptionByIdQuery(Guid optionId)
    {
        OptionId = optionId;
    }

    public Guid OptionId { get; }
}

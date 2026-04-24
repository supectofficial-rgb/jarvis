namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByStatus;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryDocumentsByStatusQuery : IQuery<GetInventoryDocumentsByStatusQueryResult>
{
    public GetInventoryDocumentsByStatusQuery(string status)
    {
        Status = status;
    }

    public string Status { get; }
}

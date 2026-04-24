namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByType;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryDocumentsByTypeQuery : IQuery<GetInventoryDocumentsByTypeQueryResult>
{
    public GetInventoryDocumentsByTypeQuery(string documentType)
    {
        DocumentType = documentType;
    }

    public string DocumentType { get; }
}

namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByNo;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryDocumentByNoQuery : IQuery<GetInventoryDocumentByNoQueryResult>
{
    public GetInventoryDocumentByNoQuery(string documentNo)
    {
        DocumentNo = documentNo;
    }

    public string DocumentNo { get; }
}

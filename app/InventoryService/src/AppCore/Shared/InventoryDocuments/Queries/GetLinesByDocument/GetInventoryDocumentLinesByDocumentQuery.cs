namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetLinesByDocument;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryDocumentLinesByDocumentQuery : IQuery<GetInventoryDocumentLinesByDocumentQueryResult>
{
    public GetInventoryDocumentLinesByDocumentQuery(Guid documentBusinessKey)
    {
        DocumentBusinessKey = documentBusinessKey;
    }

    public Guid DocumentBusinessKey { get; set; }
}

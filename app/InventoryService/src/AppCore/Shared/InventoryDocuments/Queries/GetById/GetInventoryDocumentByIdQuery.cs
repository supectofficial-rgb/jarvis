namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryDocumentByIdQuery : IQuery<GetInventoryDocumentByIdQueryResult>
{
    public GetInventoryDocumentByIdQuery(Guid documentId)
    {
        DocumentId = documentId;
    }

    public Guid DocumentId { get; }
}

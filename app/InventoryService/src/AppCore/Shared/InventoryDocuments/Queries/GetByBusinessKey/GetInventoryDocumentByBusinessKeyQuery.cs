namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryDocumentByBusinessKeyQuery : IQuery<GetInventoryDocumentByBusinessKeyQueryResult>
{
    public GetInventoryDocumentByBusinessKeyQuery(Guid documentBusinessKey)
    {
        DocumentBusinessKey = documentBusinessKey;
    }

    public Guid DocumentBusinessKey { get; }
}

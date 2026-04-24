namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.PostInventoryDocument;

using OysterFx.AppCore.Shared.Commands;

public class PostInventoryDocumentCommand : ICommand<PostInventoryDocumentCommandResult>
{
    public Guid DocumentBusinessKey { get; set; }
    public string? PostedBy { get; set; }
}

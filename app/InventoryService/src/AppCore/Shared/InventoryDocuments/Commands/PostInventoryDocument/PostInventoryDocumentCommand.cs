namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.PostInventoryDocument;

using OysterFx.AppCore.Shared.Commands;

public class PostInventoryDocumentCommand : ICommand<PostInventoryDocumentCommandResult>
{
    public Guid DocumentBusinessKey { get; set; }
    public string? PostedBy { get; set; }
    public List<PostInventoryDocumentLineSerialSelectionItem> LineSerials { get; set; } = new();
}

public sealed class PostInventoryDocumentLineSerialSelectionItem
{
    public Guid DocumentLineBusinessKey { get; set; }
    public bool UseUniqueSerialItems { get; set; }
    public List<PostInventoryDocumentLineSerialItem> Serials { get; set; } = new();
}

public sealed class PostInventoryDocumentLineSerialItem
{
    public Guid? SerialRef { get; set; }
    public string SerialNo { get; set; } = string.Empty;
}

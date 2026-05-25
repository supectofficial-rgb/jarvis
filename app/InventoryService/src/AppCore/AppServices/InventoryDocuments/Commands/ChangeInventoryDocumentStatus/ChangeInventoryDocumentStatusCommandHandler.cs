namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.ChangeInventoryDocumentStatus;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.ChangeInventoryDocumentStatus;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.PostInventoryDocument;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using OysterFx.AppCore.Shared.Commands;

public class ChangeInventoryDocumentStatusCommandHandler : CommandHandler<ChangeInventoryDocumentStatusCommand, bool>
{
    private readonly IInventoryDocumentCommandRepository _repository;
    private readonly ICommandHandler<PostInventoryDocumentCommand, PostInventoryDocumentCommandResult> _postHandler;

    public ChangeInventoryDocumentStatusCommandHandler(
        IInventoryDocumentCommandRepository repository,
        ICommandHandler<PostInventoryDocumentCommand, PostInventoryDocumentCommandResult> postHandler)
    {
        _repository = repository;
        _postHandler = postHandler;
    }

    public override async Task<CommandResult<bool>> Handle(ChangeInventoryDocumentStatusCommand command)
    {
        if (command.DocumentBusinessKey == Guid.Empty)
            return Fail("DocumentBusinessKey is required.");

        var action = (command.Action ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(action))
            return Fail("Action is required.");

        if (action == "post")
        {
            var postResult = await _postHandler.Handle(new PostInventoryDocumentCommand
            {
                DocumentBusinessKey = command.DocumentBusinessKey,
                PostedBy = command.Actor,
                LineSerials = command.LineSerials ?? new List<PostInventoryDocumentLineSerialSelectionItem>()
            });

            if (!postResult.IsSuccess)
                return Fail(postResult.ErrorMessages.FirstOrDefault() ?? "Posting inventory document failed.");

            return Ok(true);
        }

        var document = await _repository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null)
            return Fail("Inventory document was not found.");

        try
        {
            switch (action)
            {
                case "approve":
                    document.Approve(command.Actor ?? "dashboard");
                    break;
                case "reject":
                    document.Reject(command.Actor ?? "dashboard", command.ReasonCode);
                    break;
                case "cancel":
                    document.Cancel(command.Actor ?? "dashboard", command.ReasonCode);
                    break;
                default:
                    return Fail("Unsupported inventory document action.");
            }
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(true);
    }
}

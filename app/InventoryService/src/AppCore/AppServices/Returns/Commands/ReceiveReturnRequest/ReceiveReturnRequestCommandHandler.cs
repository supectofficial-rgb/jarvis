namespace Insurance.InventoryService.AppCore.AppServices.Returns.Commands.ReceiveReturnRequest;

using Insurance.InventoryService.AppCore.Shared.Returns.Commands;
using Insurance.InventoryService.AppCore.Shared.Returns.Commands.ReceiveReturnRequest;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ReceiveReturnRequestCommandHandler : CommandHandler<ReceiveReturnRequestCommand, Guid>
{
    private readonly IReturnRequestCommandRepository _repository;

    public ReceiveReturnRequestCommandHandler(IReturnRequestCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(ReceiveReturnRequestCommand command)
    {
        if (command.ReturnRequestBusinessKey == Guid.Empty)
            return Fail("ReturnRequestBusinessKey is required.");

        var request = await _repository.GetByBusinessKeyAsync(command.ReturnRequestBusinessKey);
        if (request is null)
            return Fail("Return request was not found.");

        try
        {
            request.MarkReceived(command.ReceivedBy);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(request.BusinessKey.Value);
    }
}

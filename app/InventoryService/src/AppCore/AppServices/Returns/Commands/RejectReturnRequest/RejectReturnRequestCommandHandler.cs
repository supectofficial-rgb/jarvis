namespace Insurance.InventoryService.AppCore.AppServices.Returns.Commands.RejectReturnRequest;

using Insurance.InventoryService.AppCore.Shared.Returns.Commands;
using Insurance.InventoryService.AppCore.Shared.Returns.Commands.RejectReturnRequest;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RejectReturnRequestCommandHandler : CommandHandler<RejectReturnRequestCommand, Guid>
{
    private readonly IReturnRequestCommandRepository _repository;

    public RejectReturnRequestCommandHandler(IReturnRequestCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(RejectReturnRequestCommand command)
    {
        if (command.ReturnRequestBusinessKey == Guid.Empty)
            return Fail("ReturnRequestBusinessKey is required.");

        var request = await _repository.GetByBusinessKeyAsync(command.ReturnRequestBusinessKey);
        if (request is null)
            return Fail("Return request was not found.");

        try
        {
            request.Reject(command.RejectedBy, command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(request.BusinessKey.Value);
    }
}

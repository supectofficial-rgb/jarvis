namespace Insurance.InventoryService.AppCore.AppServices.Returns.Commands.ApproveReturnRequest;

using Insurance.InventoryService.AppCore.Shared.Returns.Commands;
using Insurance.InventoryService.AppCore.Shared.Returns.Commands.ApproveReturnRequest;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ApproveReturnRequestCommandHandler : CommandHandler<ApproveReturnRequestCommand, Guid>
{
    private readonly IReturnRequestCommandRepository _repository;

    public ApproveReturnRequestCommandHandler(IReturnRequestCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(ApproveReturnRequestCommand command)
    {
        if (command.ReturnRequestBusinessKey == Guid.Empty)
            return Fail("ReturnRequestBusinessKey is required.");

        var request = await _repository.GetByBusinessKeyAsync(command.ReturnRequestBusinessKey);
        if (request is null)
            return Fail("Return request was not found.");

        try
        {
            request.Approve(command.ApprovedBy);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(request.BusinessKey.Value);
    }
}

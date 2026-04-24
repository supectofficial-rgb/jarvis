namespace Insurance.DynamicStructureService.AppCore.AppServices.Forms.Commands.CreateForm;

using Insurance.DynamicStructureService.AppCore.Domain.Forms.Entities;
using Insurance.DynamicStructureService.AppCore.Shared.Forms.Commands;
using Insurance.DynamicStructureService.AppCore.Shared.Forms.Commands.CreateForm;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class CreateFormCommandHandler(IFormCommandRepository formCommandRepository)
    : CommandHandler<CreateFormCommand, CreateFormCommandResult>
{
    private readonly IFormCommandRepository _formCommandRepository = formCommandRepository;

    public override async Task<CommandResult<CreateFormCommandResult>> Handle(CreateFormCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Title))
            return Fail("Title is required.");

        var form = Form.Create(
            title: command.Title.Trim(),
            listUrl: string.Empty,
            customData: string.Empty,
            relationCondition: string.Empty,
            authMethod: string.Empty,
            isForThird: 0,
            thirdIdMainFlowId: 0,
            submitLimitationMinute: 0,
            zaribWidth: string.Empty,
            zaribHeight: string.Empty,
            pdfBase: string.Empty,
            pdf: string.Empty,
            image: string.Empty,
            imageWidth: string.Empty,
            imageHeight: string.Empty,
            priority: 0,
            processName: string.Empty,
            userId: command.UserId,
            formTypeId: command.FormTypeId,
            formParentId: null,
            processId: null);

        await _formCommandRepository.InsertAsync(form);
        await _formCommandRepository.CommitAsync();

        return await OkAsync(new CreateFormCommandResult
        {
            BusinessKey = form.BusinessKey.Value,
            Title = form.Title
        });
    }
}



namespace Insurance.DynamicStructureService.AppCore.AppServices.FormTypes.Commands.CreateFormType;

using Insurance.DynamicStructureService.AppCore.Domain.FormTypes.Entities;
using Insurance.DynamicStructureService.AppCore.Shared.FormTypes.Commands;
using Insurance.DynamicStructureService.AppCore.Shared.FormTypes.Commands.CreateFormType;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class CreateFormTypeCommandHandler(IFormTypeCommandRepository formTypeCommandRepository)
    : CommandHandler<CreateFormTypeCommand, CreateFormTypeCommandResult>
{
    private readonly IFormTypeCommandRepository _formTypeCommandRepository = formTypeCommandRepository;

    public override async Task<CommandResult<CreateFormTypeCommandResult>> Handle(CreateFormTypeCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Title))
            return Fail("Title is required.");

        var formType = FormType.Create(command.Title.Trim());
        await _formTypeCommandRepository.InsertAsync(formType);
        await _formTypeCommandRepository.CommitAsync();

        return await OkAsync(new CreateFormTypeCommandResult
        {
            BusinessKey = formType.BusinessKey.Value,
            Title = formType.Title
        });
    }
}



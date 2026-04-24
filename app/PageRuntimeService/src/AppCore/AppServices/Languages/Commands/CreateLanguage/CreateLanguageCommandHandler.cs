namespace Insurance.PageRuntimeService.AppCore.AppServices.Languages.Commands.CreateLanguage;

using Insurance.PageRuntimeService.AppCore.Domain.Languages.Entities;
using Insurance.PageRuntimeService.AppCore.Shared.Languages.Commands;
using Insurance.PageRuntimeService.AppCore.Shared.Languages.Commands.CreateLanguage;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class CreateLanguageCommandHandler(ILanguageCommandRepository languageCommandRepository)
    : CommandHandler<CreateLanguageCommand, CreateLanguageCommandResult>
{
    private readonly ILanguageCommandRepository _languageCommandRepository = languageCommandRepository;

    public override async Task<CommandResult<CreateLanguageCommandResult>> Handle(CreateLanguageCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Title))
            return Fail("Title is required.");

        var language = Language.Create(command.Title.Trim());
        await _languageCommandRepository.InsertAsync(language);
        await _languageCommandRepository.CommitAsync();

        return await OkAsync(new CreateLanguageCommandResult
        {
            BusinessKey = language.BusinessKey.Value,
            Title = language.Title
        });
    }
}



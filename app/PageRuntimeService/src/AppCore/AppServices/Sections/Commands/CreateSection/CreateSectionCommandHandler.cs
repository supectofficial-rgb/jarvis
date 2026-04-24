namespace Insurance.PageRuntimeService.AppCore.AppServices.Sections.Commands.CreateSection;

using Insurance.PageRuntimeService.AppCore.Domain.Sections.Entities;
using Insurance.PageRuntimeService.AppCore.Shared.Sections.Commands;
using Insurance.PageRuntimeService.AppCore.Shared.Sections.Commands.CreateSection;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class CreateSectionCommandHandler(ISectionCommandRepository sectionCommandRepository)
    : CommandHandler<CreateSectionCommand, CreateSectionCommandResult>
{
    private readonly ISectionCommandRepository _sectionCommandRepository = sectionCommandRepository;

    public override async Task<CommandResult<CreateSectionCommandResult>> Handle(CreateSectionCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Url))
            return Fail("Url is required.");

        if (string.IsNullOrWhiteSpace(command.Title))
            return Fail("Title is required.");

        var section = Section.Create(
            url: command.Url.Trim(),
            title: command.Title.Trim(),
            description: command.Description?.Trim() ?? string.Empty,
            metaData: string.Empty,
            schema: string.Empty,
            metaTitle: string.Empty,
            image: string.Empty,
            writer: string.Empty,
            closed: "0",
            buttonText: string.Empty,
            date: DateTime.UtcNow,
            languageId: command.LanguageId,
            sectionTypeId: null,
            sectionLayoutId: null,
            categoryId: null);

        await _sectionCommandRepository.InsertAsync(section);
        await _sectionCommandRepository.CommitAsync();

        return await OkAsync(new CreateSectionCommandResult
        {
            BusinessKey = section.BusinessKey.Value,
            Url = section.Url
        });
    }
}



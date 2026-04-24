namespace Insurance.PageRuntimeService.Infra.Persistence.RDB.Commands.PageRuntime;

using Insurance.PageRuntimeService.AppCore.Domain.Languages.Entities;
using Insurance.PageRuntimeService.AppCore.Shared.Languages.Commands;
using OysterFx.Infra.Persistence.RDB.Commands;

public sealed class LanguageCommandRepository(PageRuntimeServiceCommandDbContext dbContext)
    : CommandRepository<Language, PageRuntimeServiceCommandDbContext>(dbContext), ILanguageCommandRepository
{
}


namespace Insurance.PageRuntimeService.Infra.Persistence.RDB.Commands.PageRuntime;

using Insurance.PageRuntimeService.AppCore.Domain.Sections.Entities;
using Insurance.PageRuntimeService.AppCore.Shared.Sections.Commands;
using OysterFx.Infra.Persistence.RDB.Commands;

public sealed class SectionCommandRepository(PageRuntimeServiceCommandDbContext dbContext)
    : CommandRepository<Section, PageRuntimeServiceCommandDbContext>(dbContext), ISectionCommandRepository
{
}


namespace Insurance.PageRuntimeService.AppCore.Shared.Sections.Commands;

using Insurance.PageRuntimeService.AppCore.Domain.Sections.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface ISectionCommandRepository : ICommandRepository<Section, long>
{
}



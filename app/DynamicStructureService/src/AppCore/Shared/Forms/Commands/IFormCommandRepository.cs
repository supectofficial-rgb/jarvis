namespace Insurance.DynamicStructureService.AppCore.Shared.Forms.Commands;

using Insurance.DynamicStructureService.AppCore.Domain.Forms.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IFormCommandRepository : ICommandRepository<Form, long>
{
    Task<Form?> GetByBusinessKeyAsync(Guid businessKey);
}



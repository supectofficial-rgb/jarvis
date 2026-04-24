namespace Insurance.DynamicStructureService.AppCore.Shared.FormTypes.Commands;

using Insurance.DynamicStructureService.AppCore.Domain.FormTypes.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface IFormTypeCommandRepository : ICommandRepository<FormType, long>
{
}



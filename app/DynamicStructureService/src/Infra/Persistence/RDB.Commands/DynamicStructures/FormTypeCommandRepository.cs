namespace Insurance.DynamicStructureService.Infra.Persistence.RDB.Commands.DynamicStructures;

using Insurance.DynamicStructureService.AppCore.Domain.FormTypes.Entities;
using Insurance.DynamicStructureService.AppCore.Shared.FormTypes.Commands;
using OysterFx.Infra.Persistence.RDB.Commands;

public sealed class FormTypeCommandRepository(DynamicStructureServiceCommandDbContext dbContext)
    : CommandRepository<FormType, DynamicStructureServiceCommandDbContext>(dbContext), IFormTypeCommandRepository
{
}


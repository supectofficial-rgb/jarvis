namespace Insurance.DynamicStructureService.Infra.Persistence.RDB.Commands.DynamicStructures;

using Insurance.DynamicStructureService.AppCore.Domain.Forms.Entities;
using Insurance.DynamicStructureService.AppCore.Shared.Forms.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public sealed class FormCommandRepository(DynamicStructureServiceCommandDbContext dbContext)
    : CommandRepository<Form, DynamicStructureServiceCommandDbContext>(dbContext), IFormCommandRepository
{
    public Task<Form?> GetByBusinessKeyAsync(Guid businessKey)
    {
        return _dbContext.Forms
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == businessKey);
    }
}



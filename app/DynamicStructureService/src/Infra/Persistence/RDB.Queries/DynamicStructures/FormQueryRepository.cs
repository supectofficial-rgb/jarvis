namespace Insurance.DynamicStructureService.Infra.Persistence.RDB.Queries.DynamicStructures;

using Insurance.DynamicStructureService.AppCore.Shared.Forms.Queries;
using Insurance.DynamicStructureService.AppCore.Shared.Forms.Queries.GetFormByBusinessKey;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public sealed class FormQueryRepository(DynamicStructureServiceQueryDbContext dbContext)
    : QueryRepository<DynamicStructureServiceQueryDbContext>(dbContext), IFormQueryRepository
{
    public async Task<GetFormByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid businessKey)
    {
        return await _dbContext.Forms
            .Where(x => x.BusinessKey == businessKey)
            .Select(x => new GetFormByBusinessKeyQueryResult
            {
                BusinessKey = x.BusinessKey,
                Title = x.Title,
                FormTypeId = x.FormTypeId,
                UserId = x.UserId
            })
            .FirstOrDefaultAsync();
    }
}


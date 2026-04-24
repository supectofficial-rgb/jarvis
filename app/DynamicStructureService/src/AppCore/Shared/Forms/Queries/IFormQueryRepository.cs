namespace Insurance.DynamicStructureService.AppCore.Shared.Forms.Queries;

using Insurance.DynamicStructureService.AppCore.Shared.Forms.Queries.GetFormByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public interface IFormQueryRepository : IQueryRepository
{
    Task<GetFormByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid businessKey);
}



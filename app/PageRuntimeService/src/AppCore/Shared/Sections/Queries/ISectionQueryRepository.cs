namespace Insurance.PageRuntimeService.AppCore.Shared.Sections.Queries;

using Insurance.PageRuntimeService.AppCore.Shared.Sections.Queries.GetSectionByUrl;
using OysterFx.AppCore.Shared.Queries;

public interface ISectionQueryRepository : IQueryRepository
{
    Task<GetSectionByUrlQueryResult?> GetByUrlAsync(string url, string? lang);
}



namespace Insurance.PageRuntimeService.Infra.Persistence.RDB.Queries.PageRuntime;

using Insurance.PageRuntimeService.AppCore.Shared.Sections.Queries;
using Insurance.PageRuntimeService.AppCore.Shared.Sections.Queries.GetSectionByUrl;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public sealed class SectionQueryRepository(PageRuntimeServiceQueryDbContext dbContext)
    : QueryRepository<PageRuntimeServiceQueryDbContext>(dbContext), ISectionQueryRepository
{
    public async Task<GetSectionByUrlQueryResult?> GetByUrlAsync(string url, string? lang)
    {
        var query = from section in _dbContext.Sections
                    join language in _dbContext.Languages on section.LanguageId equals language.Id
                    where section.Url == url
                    select new { section, language };

        if (!string.IsNullOrWhiteSpace(lang))
            query = query.Where(x => x.language.Title == lang);

        return await query
            .Select(x => new GetSectionByUrlQueryResult
            {
                BusinessKey = x.section.BusinessKey,
                Url = x.section.Url,
                Title = x.section.Title,
                Description = x.section.Description,
                Language = x.language.Title
            })
            .FirstOrDefaultAsync();
    }
}


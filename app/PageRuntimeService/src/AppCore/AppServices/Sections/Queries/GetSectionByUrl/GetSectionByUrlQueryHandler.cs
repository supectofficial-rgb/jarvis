namespace Insurance.PageRuntimeService.AppCore.AppServices.Sections.Queries.GetSectionByUrl;

using Insurance.PageRuntimeService.AppCore.Shared.Sections.Queries;
using Insurance.PageRuntimeService.AppCore.Shared.Sections.Queries.GetSectionByUrl;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public sealed class GetSectionByUrlQueryHandler(ISectionQueryRepository sectionQueryRepository)
    : QueryHandler<GetSectionByUrlQuery, GetSectionByUrlQueryResult>
{
    private readonly ISectionQueryRepository _sectionQueryRepository = sectionQueryRepository;

    public override async Task<QueryResult<GetSectionByUrlQueryResult>> ExecuteAsync(GetSectionByUrlQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            return QueryResult<GetSectionByUrlQueryResult>.Fail("Url is required.");

        var section = await _sectionQueryRepository.GetByUrlAsync(request.Url.Trim(), request.Lang);
        if (section is null)
            return QueryResult<GetSectionByUrlQueryResult>.Fail("Section was not found.", "NOT_FOUND");

        return QueryResult<GetSectionByUrlQueryResult>.Success(section);
    }
}



namespace Insurance.DynamicStructureService.AppCore.AppServices.Forms.Queries.GetFormByBusinessKey;

using Insurance.DynamicStructureService.AppCore.Shared.Forms.Queries;
using Insurance.DynamicStructureService.AppCore.Shared.Forms.Queries.GetFormByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public sealed class GetFormByBusinessKeyQueryHandler(IFormQueryRepository formQueryRepository)
    : QueryHandler<GetFormByBusinessKeyQuery, GetFormByBusinessKeyQueryResult>
{
    private readonly IFormQueryRepository _formQueryRepository = formQueryRepository;

    public override async Task<QueryResult<GetFormByBusinessKeyQueryResult>> ExecuteAsync(GetFormByBusinessKeyQuery request)
    {
        var form = await _formQueryRepository.GetByBusinessKeyAsync(request.FormBusinessKey);
        if (form is null)
            return QueryResult<GetFormByBusinessKeyQueryResult>.Fail("Form was not found.", "NOT_FOUND");

        return QueryResult<GetFormByBusinessKeyQueryResult>.Success(form);
    }
}



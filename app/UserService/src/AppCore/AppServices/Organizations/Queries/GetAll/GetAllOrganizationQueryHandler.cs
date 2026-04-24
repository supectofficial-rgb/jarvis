namespace Insurance.UserService.AppCore.AppServices.Organizations.Queries.GetAll;

using Insurance.UserService.AppCore.Shared.Organizations.Queries;
using Insurance.UserService.AppCore.Shared.Organizations.Queries.GetAll;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GetAllOrganizationQueryHandler(IOrganizationQueryRepository organizationQueryRepository) : QueryHandler<GetAllOrganizationQuery, IEnumerable<GetAllOrganizationQueryResult?>>
{
    private readonly IOrganizationQueryRepository _organizationQueryRepository = organizationQueryRepository;

    public override async Task<QueryResult<IEnumerable<GetAllOrganizationQueryResult?>>> ExecuteAsync(GetAllOrganizationQuery request)
    {
        var queryResult = await _organizationQueryRepository.QueryAsync(request);
        return await AsQueryResult(queryResult);
    }
}
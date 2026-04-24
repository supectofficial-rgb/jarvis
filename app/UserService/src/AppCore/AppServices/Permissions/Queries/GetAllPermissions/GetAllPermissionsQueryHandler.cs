namespace Insurance.UserService.AppCore.AppServices.Permissions.Queries.GetAllPermissions;

using Insurance.UserService.AppCore.Shared.Permissions.Queries;
using Insurance.UserService.AppCore.Shared.Permissions.Queries.GetAll;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetAllPermissionsQueryHandler(IPermissionQueryRepository permissionQueryRepository) : QueryHandler<GetAllPermissionsQuery, IEnumerable<GetAllPermissionsQueryResult?>>
{
    private readonly IPermissionQueryRepository _permissionQueryRepository = permissionQueryRepository;

    public override async Task<QueryResult<IEnumerable<GetAllPermissionsQueryResult?>>> ExecuteAsync(GetAllPermissionsQuery request)
    {
        var queryResult = await _permissionQueryRepository.QueryAsync(request);
        return await AsQueryResult(queryResult);
    }
}
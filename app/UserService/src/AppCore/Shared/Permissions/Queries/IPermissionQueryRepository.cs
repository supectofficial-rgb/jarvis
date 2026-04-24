namespace Insurance.UserService.AppCore.Shared.Permissions.Queries;

using Insurance.UserService.AppCore.Shared.Permissions.Queries.GetAll;
using OysterFx.AppCore.Shared.Queries;

public interface IPermissionQueryRepository : IQueryRepository
{
    public Task<IEnumerable<GetAllPermissionsQueryResult?>> QueryAsync(GetAllPermissionsQuery query);
}
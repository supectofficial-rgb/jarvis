namespace Insurance.UserService.Infra.Persistence.RDB.Queries.Permissions;

using Insurance.UserService.AppCore.Shared.Permissions.Queries;
using Insurance.UserService.AppCore.Shared.Permissions.Queries.GetAll;
using OysterFx.Infra.Persistence.RDB.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PermissionQueryRepository : QueryRepository<InsuranceUserServiceQueryDbContext>, IPermissionQueryRepository
{
    public PermissionQueryRepository(InsuranceUserServiceQueryDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<GetAllPermissionsQueryResult?>> QueryAsync(GetAllPermissionsQuery query)
    {
        var permissionQuery = _dbContext.Permissions;

        var queryResult = (from p in permissionQuery
                           select new GetAllPermissionsQueryResult()
                           {
                               BusinessKey = p.BusinessKey,
                               Code = p.Code
                           })?.AsEnumerable();
        return queryResult!;
    }
}
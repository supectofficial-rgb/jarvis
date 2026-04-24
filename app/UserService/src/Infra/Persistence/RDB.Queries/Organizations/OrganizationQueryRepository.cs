namespace Insurance.UserService.Infra.Persistence.RDB.Queries.Organizations;

using Insurance.UserService.AppCore.Shared.Organizations.Queries;
using Insurance.UserService.AppCore.Shared.Organizations.Queries.GetAll;
using OysterFx.Infra.Persistence.RDB.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrganizationQueryRepository : QueryRepository<InsuranceUserServiceQueryDbContext>, IOrganizationQueryRepository
{
    public OrganizationQueryRepository(InsuranceUserServiceQueryDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<GetAllOrganizationQueryResult?>> QueryAsync(GetAllOrganizationQuery query)
    {
        var organizationQuery = _dbContext.Organizations;

        var queryResult = (from org in organizationQuery
                           select new GetAllOrganizationQueryResult()
                           {
                               BusinessKey = org.BusinessKey,
                               Name = org.Name
                           })?.AsEnumerable();
        return queryResult!;
    }
}
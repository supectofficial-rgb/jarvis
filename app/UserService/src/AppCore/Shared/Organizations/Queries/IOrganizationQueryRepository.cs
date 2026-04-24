namespace Insurance.UserService.AppCore.Shared.Organizations.Queries;

using Insurance.UserService.AppCore.Shared.Organizations.Queries.GetAll;
using OysterFx.AppCore.Shared.Queries;

public interface IOrganizationQueryRepository : IQueryRepository
{
    Task<IEnumerable<GetAllOrganizationQueryResult?>> QueryAsync(GetAllOrganizationQuery query);
}
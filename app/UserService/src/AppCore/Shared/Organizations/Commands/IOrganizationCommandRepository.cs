namespace Insurance.UserService.AppCore.Shared.Organizations.Commands;

using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using OysterFx.AppCore.Domain.ValueObjects;

public interface IOrganizationCommandRepository
{
    Organization Get(BusinessKey businessKey);
    Task<Organization> GetAsync(BusinessKey businessKey);
    Task InsertAsync(Organization entity);
    Task<int> CommitAsync();
}
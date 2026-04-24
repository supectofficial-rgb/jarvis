namespace Insurance.UserService.AppCore.Shared.Roles.Commands;

using Insurance.UserService.AppCore.Domain.Roles.Entities;
using OysterFx.AppCore.Domain.ValueObjects;

public interface IRoleCommandRepository
{
    AppRole Get(BusinessKey BusinessKey);
    Task<AppRole> GetAsync(BusinessKey BusinessKey);
    Task InsertAsync(AppRole entity);
    Task<int> CommitAsync();
}
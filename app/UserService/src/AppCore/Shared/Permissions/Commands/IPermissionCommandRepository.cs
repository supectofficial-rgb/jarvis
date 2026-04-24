namespace Insurance.UserService.AppCore.Shared.Permissions.Commands;

using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using OysterFx.AppCore.Domain.ValueObjects;

public interface IPermissionCommandRepository
{
    Permission Get(BusinessKey BusinessKey);
    Task<Permission> GetAsync(BusinessKey BusinessKey);
    Task InsertAsync(Permission entity);
    Task<int> CommitAsync();
}
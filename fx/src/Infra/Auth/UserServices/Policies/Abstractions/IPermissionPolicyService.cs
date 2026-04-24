namespace OysterFx.Infra.Auth.UserServices.Policies.Abstractions;

using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

public interface IPermissionPolicyService
{
    Task RegisterAllPermissionsAsync(AuthorizationOptions options);
}
namespace Insurance.UserService.Infra.Persistence.RDB.Commands.ValueConversions;

using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class TenantIdConversion : ValueConverter<TenantId, string>
{
    public TenantIdConversion()
        : base(
            tenantId => tenantId != null ? tenantId.Value : null,          // TenantId -> string for DB
            value => !string.IsNullOrEmpty(value) ? TenantId.FromString(value) : null // string from DB -> TenantId
        )
    { }
}
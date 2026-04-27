namespace Insurance.UserService.Infra.Persistence.RDB.Commands;

using Insurance.UserService.AppCore.Domain.Memberships.Entities;
using Insurance.UserService.AppCore.Domain.Organizations.Entities;
using Insurance.UserService.AppCore.Domain.Permissions.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Enums;
using Insurance.UserService.AppCore.Domain.Tenants.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Domain.Users.Enums;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using System;
using System.Linq;

public static class SeedData
{
    private static readonly Guid SystemOrgBusinessKey = Guid.Parse("27632bf0-4ab2-4aa6-a606-b1e5d0135258");

    private static readonly Guid User1BusinessKey = Guid.Parse("ef58349f-d52c-4c5c-9966-0ec7c14f3c51");

    private static readonly Guid Permission1BusinessKey = Guid.Parse("3dd4ec06-a23c-41fd-acc2-62ffe2bc7ea6");
    private static readonly Guid Permission2BusinessKey = Guid.Parse("0a9d0ce0-ab62-4fd0-b8fb-aa097e0882c7");
    private static readonly Guid Permission3BusinessKey = Guid.Parse("82514bf2-ebb6-4e75-84a5-9c4d8831121d");
    private static readonly Guid Permission4BusinessKey = Guid.Parse("874b7fe9-b8b6-41f5-b00c-b611ede1634e");
    private static readonly Guid Permission5BusinessKey = Guid.Parse("6a3598c4-5e73-4974-b7cf-28eddcab882e");
    private static readonly Guid Permission6BusinessKey = Guid.Parse("2a4e4933-1d95-47da-898f-42dd4ed2604d");
    private static readonly Guid Permission7BusinessKey = Guid.Parse("ebf46703-c5be-47ea-b920-3d936d2d3746");
    private static readonly Guid Permission8BusinessKey = Guid.Parse("5051e245-da78-4c0c-b86e-3eeabab0cffe");
    private static readonly Guid Permission9BusinessKey = Guid.Parse("b09632e4-467f-4b52-82a5-9e6a61e6c7e1");
    private static readonly Guid Permission10BusinessKey = Guid.Parse("5f491ad7-77f1-444f-9501-1bd7a39bc14f");

    private static readonly Guid Role1BusinessKey = Guid.Parse("37f9bd6a-4a12-4ddf-9410-0be0d59c8e96");

    private static readonly DateTime FixedDate = new DateTime(2026, 2, 23, 22, 5, 5, DateTimeKind.Utc);

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedOrganization(modelBuilder);
        SeedPermissions(modelBuilder);
        SeedRoles(modelBuilder);
        SeedUsers(modelBuilder);
        SeedUserMemberships(modelBuilder);
        SeedRolePermissions(modelBuilder);
    }

    private static void SeedOrganization(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>().HasData(
            new
            {
                Id = 1L,
                BusinessKey = "27632bf0-4ab2-4aa6-a606-b1e5d0135258",
                TenantId = $"TENANT_{SystemOrgBusinessKey}",
                Name = "شرکت نمونه داده‌پرداز",
                IsActive = true
            });
    }

    private static void SeedPermissions(ModelBuilder modelBuilder)
    {
        var permissions = new[]
        {
            new { Id = 1L, BusinessKey = BusinessKey.FromGuid(Permission1BusinessKey), Code = "Role.Create", Scope = RoleScope.System, Description = "ایجاد نقش", IsActive = true },
            new { Id = 2L, BusinessKey = BusinessKey.FromGuid(Permission2BusinessKey), Code = "Role.AssignPermission", Scope = RoleScope.System, Description = "تخصیص دسترسی به نقش", IsActive = true },
            new { Id = 3L, BusinessKey = BusinessKey.FromGuid(Permission3BusinessKey), Code = "Role.RemovePermission", Scope = RoleScope.System, Description = "حذف دسترسی از نقش", IsActive = true },
            new { Id = 4L, BusinessKey = BusinessKey.FromGuid(Permission4BusinessKey), Code = "Permission.Create.System", Scope = RoleScope.System, Description = "ایجاد دسترسی سیستمی", IsActive = true },
            new { Id = 5L, BusinessKey = BusinessKey.FromGuid(Permission5BusinessKey), Code = "Organization.Create", Scope = RoleScope.System, Description = "تعریف سازمان", IsActive = true },
            new { Id = 6L, BusinessKey = BusinessKey.FromGuid(Permission6BusinessKey), Code = "Organization.GetAll", Scope = RoleScope.System, Description = "مشاهده لیست سازمانها", IsActive = true },
            new { Id = 7L, BusinessKey = BusinessKey.FromGuid(Permission7BusinessKey), Code = "Membership.Create", Scope = RoleScope.System, Description = "ایجاد Membership", IsActive = true },
            new { Id = 8L, BusinessKey = BusinessKey.FromGuid(Permission8BusinessKey), Code = "Membership.AssignRole", Scope = RoleScope.System, Description = "تخصیص نقش به Membership", IsActive = true },
            new { Id = 9L, BusinessKey = BusinessKey.FromGuid(Permission9BusinessKey), Code = "Auth.Register", Scope = RoleScope.System, Description = "ثبت نام", IsActive = true },
            new { Id = 10L, BusinessKey = BusinessKey.FromGuid(Permission10BusinessKey), Code = "User.Create", Scope = RoleScope.System, Description = "تعریف کاربر", IsActive = true }
        };

        modelBuilder.Entity<Permission>().HasData(
            permissions.Select(p => new
            {
                p.Id,
                p.BusinessKey,
                p.Code,
                p.Scope,
                p.Description,
                p.IsActive,
                CreatedByUserId = (string?)null,
                CreatedDateTime = (DateTime?)null,
                ModifiedByUserId = (string?)null,
                ModifiedDateTime = (DateTime?)null
            }).ToArray()
        );
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        var tenantId = TenantId.FromString($"TENANT_{SystemOrgBusinessKey}");

        var roles = new[]
        {
        new
        {
            Id = 1L,
            BusinessKey = BusinessKey.FromGuid(Role1BusinessKey),
            Name = "مدیر سیستم",
            Scope = RoleScope.System,
            NormalizedName = "مدیر سیستم",
            TenantId = tenantId,
            ConcurrencyStamp = (string?)null
        }
    };

        modelBuilder.Entity<AppRole>().HasData(roles);
    }
    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        // 1qaz!QAZ
        const string passwordHash = "AQAAAAIAAYagAAAAEGi5xfPdoTX8bPTrU/LYgT7k5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Q==";

        var users = new[]
        {
        new
        {
            Id = 1L,
            BusinessKey = BusinessKey.FromGuid(User1BusinessKey),
            Email = "admin@example.com",
            UserName = "admin",
            Code = "AD001",
            FullName = "مدیر ارشد",
            EmailConfirmed = true,
            PasswordHash = passwordHash,
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            NormalizedUserName = "ADMIN",
            AccessFailedCount = 0,
            LockoutEnabled = false,
            LockoutEnd = (DateTimeOffset?)null,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false,
            PhoneNumber = (string?)null,
            PhoneNumberConfirmed = false,
            MaxAllowedExpertiseAmount = 0m,
            MaxAllowedDailyCaseReferral = 0m,
            MaxAllowedOpenCases = 0m,
            City = "null",
            Province = "null",
            GPSDeviceId = "null",
            MobileNumber = "null",
            ProfileImageBase64 = "null",
            UserExpirationDate = "null",
            PasswordExpirationDate = "null"
        }
    };

        modelBuilder.Entity<User>().HasData(users);
    }

    private static void SeedUserMemberships(ModelBuilder modelBuilder)
    {
        var tenantId = TenantId.FromString($"TENANT_{SystemOrgBusinessKey}");
        var userMemberships = new[]
        {
            new { Id = 1L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), UserBusinessKey = BusinessKey.FromGuid(User1BusinessKey),
                OrganizationBusinessKey = BusinessKey.FromGuid(SystemOrgBusinessKey), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey),
                AssignmentType = AssignmentType.Direct, StartDateTime  = FixedDate, IsActive = true, TenantId = tenantId }
        };

        modelBuilder.Entity<Membership>().HasData(userMemberships);
    }

    private static void SeedRolePermissions(ModelBuilder modelBuilder)
    {
        var rolePermissions = new[]
        {
            new { Id = 1L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission1BusinessKey) },
            new { Id = 2L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission2BusinessKey) },
            new { Id = 3L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission3BusinessKey) },
            new { Id = 4L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission4BusinessKey) },
            new { Id = 5L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission5BusinessKey) },
            new { Id = 6L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission6BusinessKey) },
            new { Id = 7L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission7BusinessKey) },
            new { Id = 8L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission8BusinessKey) },
            new { Id = 9L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission9BusinessKey) },
            new { Id = 10L, BusinessKey = BusinessKey.FromGuid(Guid.NewGuid()), RoleBusinessKey = BusinessKey.FromGuid(Role1BusinessKey), PermissionBusinessKey = BusinessKey.FromGuid(Permission10BusinessKey) }
        };

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
    }
}

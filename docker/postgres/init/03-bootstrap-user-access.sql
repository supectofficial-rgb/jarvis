BEGIN;

-- Bootstrap RBAC data.
-- Password for both bootstrap accounts is intended to be changed immediately.
-- Current password hash is for: 1qaz!QAZ

INSERT INTO "Permissions" ("Id", "Code", "Scope", "Description", "IsActive", "BusinessKey")
VALUES
    (1000, '*', 1, 'Full system access', true, '00000000-0000-0000-0000-000000001000'),
    (1001, '*', 2, 'Full organization access', true, '00000000-0000-0000-0000-000000001001'),

    (1100, 'Panel.Dashboard.View', 2, 'View dashboard panel', true, '00000000-0000-0000-0000-000000001100'),
    (1101, 'Panel.PeopleManagement.View', 2, 'View people management panel', true, '00000000-0000-0000-0000-000000001101'),
    (1102, 'Panel.Catalog.View', 2, 'View catalog management panel', true, '00000000-0000-0000-0000-000000001102'),
    (1103, 'Panel.Inventory.View', 2, 'View inventory management panel', true, '00000000-0000-0000-0000-000000001103'),

    (1200, 'Catalog.Category.View', 2, 'View catalog categories', true, '00000000-0000-0000-0000-000000001200'),
    (1201, 'Catalog.Category.Create', 2, 'Create catalog categories', true, '00000000-0000-0000-0000-000000001201'),
    (1202, 'Catalog.Category.Update', 2, 'Update catalog categories', true, '00000000-0000-0000-0000-000000001202'),
    (1203, 'Catalog.Category.Delete', 2, 'Delete catalog categories', true, '00000000-0000-0000-0000-000000001203'),
    (1204, 'Catalog.Category.Move', 2, 'Move catalog categories', true, '00000000-0000-0000-0000-000000001204'),
    (1210, 'Catalog.Product.View', 2, 'View catalog products', true, '00000000-0000-0000-0000-000000001210'),
    (1211, 'Catalog.Product.Create', 2, 'Create catalog products', true, '00000000-0000-0000-0000-000000001211'),
    (1212, 'Catalog.Product.Update', 2, 'Update catalog products', true, '00000000-0000-0000-0000-000000001212'),
    (1213, 'Catalog.Product.Delete', 2, 'Delete catalog products', true, '00000000-0000-0000-0000-000000001213'),
    (1214, 'Catalog.Product.ChangeCategory', 2, 'Change product category', true, '00000000-0000-0000-0000-000000001214'),
    (1220, 'Catalog.Variant.View', 2, 'View product variants', true, '00000000-0000-0000-0000-000000001220'),
    (1221, 'Catalog.Variant.Create', 2, 'Create product variants', true, '00000000-0000-0000-0000-000000001221'),
    (1222, 'Catalog.Variant.Update', 2, 'Update product variants', true, '00000000-0000-0000-0000-000000001222'),
    (1223, 'Catalog.Variant.Delete', 2, 'Delete product variants', true, '00000000-0000-0000-0000-000000001223'),
    (1224, 'Catalog.Variant.ChangeTrackingPolicy', 2, 'Change variant tracking policy', true, '00000000-0000-0000-0000-000000001224'),
    (1225, 'Catalog.Variant.ChangeBaseUom', 2, 'Change variant base unit', true, '00000000-0000-0000-0000-000000001225'),
    (1226, 'Catalog.Variant.LockInventoryMovement', 2, 'Lock variant inventory movement', true, '00000000-0000-0000-0000-000000001226'),

    (1300, 'Inventory.Product.Read', 3, 'Read products through API', true, '00000000-0000-0000-0000-000000001300'),
    (1301, 'Inventory.Product.Create', 3, 'Create products through API', true, '00000000-0000-0000-0000-000000001301'),
    (1302, 'Inventory.Product.Update', 3, 'Update products through API', true, '00000000-0000-0000-0000-000000001302'),
    (1303, 'Inventory.Product.Delete', 3, 'Delete products through API', true, '00000000-0000-0000-0000-000000001303'),
    (1310, 'Inventory.ProductVariant.Read', 3, 'Read product variants through API', true, '00000000-0000-0000-0000-000000001310'),
    (1311, 'Inventory.ProductVariant.Create', 3, 'Create product variants through API', true, '00000000-0000-0000-0000-000000001311'),
    (1312, 'Inventory.ProductVariant.Update', 3, 'Update product variants through API', true, '00000000-0000-0000-0000-000000001312'),
    (1313, 'Inventory.ProductVariant.Delete', 3, 'Delete product variants through API', true, '00000000-0000-0000-0000-000000001313'),
    (1320, 'Inventory.Category.Create', 3, 'Create categories through API', true, '00000000-0000-0000-0000-000000001320'),
    (1321, 'Inventory.Category.Update', 3, 'Update categories through API', true, '00000000-0000-0000-0000-000000001321'),
    (1322, 'Inventory.Category.Delete', 3, 'Delete categories through API', true, '00000000-0000-0000-0000-000000001322')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "Organizations" ("Id", "TenantId", "Name", "IsActive", "BusinessKey")
VALUES
    (1000, 'TENANT_00000000-0000-0000-0000-000000002000', 'System', true, '00000000-0000-0000-0000-000000002000'),
    (1001, 'TENANT_00000000-0000-0000-0000-000000002001', 'Nivad', true, '00000000-0000-0000-0000-000000002001')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "Roles" ("Id", "TenantId", "BusinessKey", "Scope", "Name", "NormalizedName", "ConcurrencyStamp")
VALUES
    (1000, 'TENANT_00000000-0000-0000-0000-000000002000', '00000000-0000-0000-0000-000000003000', 1, 'SystemAdmin', 'SYSTEMADMIN', '00000000-0000-0000-0000-000000003000'),
    (1001, 'TENANT_00000000-0000-0000-0000-000000002001', '00000000-0000-0000-0000-000000003001', 2, 'NivadOrganizationAdmin', 'NIVADORGANIZATIONADMIN', '00000000-0000-0000-0000-000000003001')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "Users" ("Id", "MobileNumber", "Code", "FullName", "City", "Province", "MaxAllowedExpertiseAmount", "MaxAllowedDailyCaseReferral", "MaxAllowedOpenCases", "BusinessKey")
VALUES
    (1000, NULL, 'SYS-ADMIN', 'System Administrator', NULL, NULL, 0, 0, 0, '00000000-0000-0000-0000-000000004000'),
    (1001, NULL, 'NIVAD-ADMIN', 'Nivad Organization Administrator', NULL, NULL, 0, 0, 0, '00000000-0000-0000-0000-000000004001')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "AspNetUsers" (
    "Id", "UserBusinessKey", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed",
    "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed",
    "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount")
VALUES
    (1000, '00000000-0000-0000-0000-000000004000', 'admin', 'ADMIN', 'admin@nivad.local', 'ADMIN@NIVAD.LOCAL', true,
     'AQAAAAIAAYagAAAAEGi5xfPdoTX8bPTrU/LYgT7k5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Q==',
     '00000000-0000-0000-0000-000000005000', '00000000-0000-0000-0000-000000006000', NULL, false, false, NULL, false, 0),
    (1001, '00000000-0000-0000-0000-000000004001', 'nivad.admin', 'NIVAD.ADMIN', 'admin@nivad.ir', 'ADMIN@NIVAD.IR', true,
     'AQAAAAIAAYagAAAAEGi5xfPdoTX8bPTrU/LYgT7k5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Z5F5Q==',
     '00000000-0000-0000-0000-000000005001', '00000000-0000-0000-0000-000000006001', NULL, false, false, NULL, false, 0)
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
VALUES
    (1000, 1000)
ON CONFLICT ("UserId", "RoleId") DO NOTHING;

INSERT INTO "Memberships" ("Id", "TenantId", "UserBusinessKey", "OrganizationBusinessKey", "StartDateTime", "IsActive", "BusinessKey")
VALUES
    (1001, 'TENANT_00000000-0000-0000-0000-000000002001', '00000000-0000-0000-0000-000000004001', '00000000-0000-0000-0000-000000002001', NOW(), true, '00000000-0000-0000-0000-000000007001')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "UserRoleAssignments" ("Id", "RoleBusinessKey", "MembershipBusinessKey", "BusinessKey")
VALUES
    (1001, '00000000-0000-0000-0000-000000003001', '00000000-0000-0000-0000-000000007001', '00000000-0000-0000-0000-000000008001')
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "RolePermissions" ("Id", "RoleBusinessKey", "PermissionBusinessKey", "AppRoleId", "BusinessKey")
VALUES
    (1000, '00000000-0000-0000-0000-000000003000', '00000000-0000-0000-0000-000000001000', 1000, '00000000-0000-0000-0000-000000009000'),
    (1001, '00000000-0000-0000-0000-000000003001', '00000000-0000-0000-0000-000000001001', 1001, '00000000-0000-0000-0000-000000009001')
ON CONFLICT ("Id") DO NOTHING;

COMMIT;

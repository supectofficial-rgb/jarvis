WITH target_role AS (
    SELECT '00000000-0000-0000-0000-000000003001'::uuid AS "RoleBusinessKey"
),
active_permissions AS (
    SELECT
        p."BusinessKey" AS "PermissionBusinessKey"
    FROM "Permissions" p
    WHERE p."IsActive" = true
)
INSERT INTO "RolePermissions"
(
    "BusinessKey",
    "RoleBusinessKey",
    "PermissionBusinessKey"
)
SELECT
    (
        substr(md5(tr."RoleBusinessKey"::text || ':' || ap."PermissionBusinessKey"::text), 1, 8) || '-' ||
        substr(md5(tr."RoleBusinessKey"::text || ':' || ap."PermissionBusinessKey"::text), 9, 4) || '-' ||
        substr(md5(tr."RoleBusinessKey"::text || ':' || ap."PermissionBusinessKey"::text), 13, 4) || '-' ||
        substr(md5(tr."RoleBusinessKey"::text || ':' || ap."PermissionBusinessKey"::text), 17, 4) || '-' ||
        substr(md5(tr."RoleBusinessKey"::text || ':' || ap."PermissionBusinessKey"::text), 21, 12)
    )::uuid AS "BusinessKey",
    tr."RoleBusinessKey",
    ap."PermissionBusinessKey"
FROM target_role tr
CROSS JOIN active_permissions ap
WHERE NOT EXISTS (
    SELECT 1
    FROM "RolePermissions" rp
    WHERE rp."RoleBusinessKey" = tr."RoleBusinessKey"
      AND rp."PermissionBusinessKey" = ap."PermissionBusinessKey"
);

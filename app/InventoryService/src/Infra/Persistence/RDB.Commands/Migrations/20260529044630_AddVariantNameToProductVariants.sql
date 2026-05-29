START TRANSACTION;
ALTER TABLE "ProductVariants" ADD "VariantName" text;

WITH default_formulas AS (
    SELECT DISTINCT ON (f."CategoryRef")
        f."CategoryRef",
        f."BusinessKey" AS "FormulaRef",
        COALESCE(NULLIF(btrim(f."Separator"), ''), ' ') AS "Separator"
    FROM "CategoryVariantNameFormulas" f
    WHERE f."IsActive" = TRUE
    ORDER BY f."CategoryRef", f."DisplayOrder", f."Name"
),
variant_name_suffixes AS (
    SELECT
        pv."BusinessKey" AS "VariantRef",
        string_agg(
            COALESCE(
                NULLIF(btrim(vav."Value"), ''),
                NULLIF(btrim(ao."Name"), ''),
                NULLIF(btrim(ao."Value"), '')
            ),
            COALESCE(df."Separator", ' ') ORDER BY fpart."SortOrder"
        ) AS "GeneratedSuffix"
    FROM "ProductVariants" pv
    JOIN "Products" p ON p."BusinessKey" = pv."ProductRef"
    JOIN "Categories" c ON c."BusinessKey" = p."CategoryRef"
    LEFT JOIN default_formulas df ON df."CategoryRef" = c."BusinessKey"
    LEFT JOIN "CategoryVariantNameFormulaParts" fpart ON fpart."FormulaRef" = df."FormulaRef"
    LEFT JOIN "VariantAttributeValues" vav
        ON vav."VariantRef" = pv."BusinessKey"
        AND vav."AttributeRef" = fpart."AttributeRef"
    LEFT JOIN "AttributeOptions" ao ON ao."BusinessKey" = vav."OptionRef"
    GROUP BY pv."BusinessKey"
)
UPDATE "ProductVariants" pv
SET "VariantName" = COALESCE((
    SELECT CASE
        WHEN s."GeneratedSuffix" IS NULL OR btrim(s."GeneratedSuffix") = '' THEN pv."VariantSku"
        WHEN c."Name" IS NULL OR btrim(c."Name") = '' THEN s."GeneratedSuffix"
        ELSE c."Name" || ' - ' || s."GeneratedSuffix"
    END
    FROM "Products" p
    JOIN "Categories" c ON c."BusinessKey" = p."CategoryRef"
    LEFT JOIN variant_name_suffixes s ON s."VariantRef" = pv."BusinessKey"
    WHERE p."BusinessKey" = pv."ProductRef"
), pv."VariantSku")
WHERE pv."VariantName" IS NULL
   OR btrim(pv."VariantName") = '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260529044630_AddVariantNameToProductVariants', '9.0.11');

COMMIT;

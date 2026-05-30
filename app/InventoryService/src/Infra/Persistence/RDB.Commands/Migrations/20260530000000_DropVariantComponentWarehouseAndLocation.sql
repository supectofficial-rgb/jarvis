START TRANSACTION;

DROP INDEX IF EXISTS "IX_VariantComponents_VariantRef_ComponentVariantRef_WarehouseRef_LocationRef";

ALTER TABLE "VariantComponents" DROP COLUMN IF EXISTS "WarehouseRef";
ALTER TABLE "VariantComponents" DROP COLUMN IF EXISTS "LocationRef";

CREATE UNIQUE INDEX "IX_VariantComponents_VariantRef_ComponentVariantRef"
ON "VariantComponents" ("VariantRef", "ComponentVariantRef");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260530000000_DropVariantComponentWarehouseAndLocation', '9.0.11');

COMMIT;

START TRANSACTION;
DROP INDEX "IX_VariantAddOns_VariantRef_AddOnVariantRef";

ALTER TABLE "VariantAddOns" ALTER COLUMN "AddOnVariantRef" DROP NOT NULL;

ALTER TABLE "VariantAddOns" ADD "IsRequired" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "VariantAddOns" ADD "TagId" uuid;

CREATE UNIQUE INDEX "IX_VariantAddOns_VariantRef_AddOnVariantRef" ON "VariantAddOns" ("VariantRef", "AddOnVariantRef") WHERE "AddOnVariantRef" IS NOT NULL;

CREATE UNIQUE INDEX "IX_VariantAddOns_VariantRef_TagId" ON "VariantAddOns" ("VariantRef", "TagId") WHERE "TagId" IS NOT NULL;

ALTER TABLE "VariantAddOns" ADD CONSTRAINT "CK_VariantAddOns_AddOnVariantRefOrTagId" CHECK ("AddOnVariantRef" IS NOT NULL OR "TagId" IS NOT NULL);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260529105816_AddVariantAddOnTagAndRequired', '9.0.11');

COMMIT;


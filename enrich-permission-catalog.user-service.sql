START TRANSACTION;
ALTER TABLE "Permissions" ALTER COLUMN "Description" TYPE character varying(500);

ALTER TABLE "Permissions" ALTER COLUMN "Code" TYPE character varying(150);

ALTER TABLE "Permissions" ADD "Module" character varying(100) NOT NULL DEFAULT '';

ALTER TABLE "Permissions" ADD "Title" character varying(200) NOT NULL DEFAULT '';

ALTER TABLE "Permissions" ADD "Type" smallint NOT NULL DEFAULT 7;

CREATE UNIQUE INDEX "IX_Permissions_Code" ON "Permissions" ("Code");

CREATE INDEX "IX_Permissions_Module" ON "Permissions" ("Module");

CREATE INDEX "IX_Permissions_Type" ON "Permissions" ("Type");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260513082834_EnrichPermissionCatalog', '9.0.11');

COMMIT;


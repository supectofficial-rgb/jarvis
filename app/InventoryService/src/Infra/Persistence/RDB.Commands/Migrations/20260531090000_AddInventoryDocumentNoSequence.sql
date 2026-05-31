BEGIN;

CREATE SEQUENCE IF NOT EXISTS "InventoryDocumentNoSequence"
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    CACHE 1;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260531090000_AddInventoryDocumentNoSequence', '9.0.11');

COMMIT;

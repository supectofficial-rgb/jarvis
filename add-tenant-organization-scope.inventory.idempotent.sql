DO $$
DECLARE
    tbl text;
    tables text[] := ARRAY[
        'Warehouses',
        'VariantUomConversions',
        'VariantImages',
        'VariantComponents',
        'VariantAttributeValues',
        'VariantAddOns',
        'UnitOfMeasures',
        'StockDetails',
        'SerialItems',
        'SellerVariantPrices',
        'Sellers',
        'ReturnRequestTransitions',
        'ReturnRequests',
        'ReturnLineSerials',
        'ReturnLines',
        'ReservationTransitions',
        'ReservationAllocations',
        'QualityStatuses',
        'ProductVariants',
        'Products',
        'ProductAttributeValues',
        'PriceTypes',
        'PriceChannels',
        'Offers',
        'Locations',
        'InventoryTransactions',
        'InventoryTransactionLines',
        'InventorySourceConsumptions',
        'InventorySourceBalances',
        'InventorySourceAllocations',
        'InventoryReservations',
        'InventoryDocuments',
        'InventoryDocumentLineSerials',
        'InventoryDocumentLines',
        'FulfillmentTransitions',
        'Fulfillments',
        'FulfillmentLineSerials',
        'FulfillmentLines',
        'CategoryVariantNameFormulas',
        'CategoryVariantNameFormulaParts',
        'CategorySchemaVersions',
        'CategoryAttributeRules',
        'Categories',
        'AttributeOptions',
        'AttributeDefinitions'
    ];
BEGIN
    FOREACH tbl IN ARRAY tables
    LOOP
        IF to_regclass(format('public.%I', tbl)) IS NOT NULL THEN
            EXECUTE format('ALTER TABLE %I ADD COLUMN IF NOT EXISTS "OrganizationBusinessKey" character varying(50)', tbl);
            EXECUTE format('ALTER TABLE %I ADD COLUMN IF NOT EXISTS "TenantId" character varying(100)', tbl);
            EXECUTE format('CREATE INDEX IF NOT EXISTS %I ON %I ("OrganizationBusinessKey")', 'IX_' || tbl || '_OrganizationBusinessKey', tbl);
            EXECUTE format('CREATE INDEX IF NOT EXISTS %I ON %I ("TenantId")', 'IX_' || tbl || '_TenantId', tbl);
        END IF;
    END LOOP;
END $$;

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260513054336_AddTenantOrganizationScope', '9.0.11')
ON CONFLICT ("MigrationId") DO NOTHING;

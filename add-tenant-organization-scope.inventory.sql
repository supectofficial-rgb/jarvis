START TRANSACTION;
ALTER TABLE "Warehouses" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Warehouses" ADD "TenantId" character varying(100);

ALTER TABLE "VariantUomConversions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "VariantUomConversions" ADD "TenantId" character varying(100);

ALTER TABLE "VariantImages" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "VariantImages" ADD "TenantId" character varying(100);

ALTER TABLE "VariantComponents" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "VariantComponents" ADD "TenantId" character varying(100);

ALTER TABLE "VariantAttributeValues" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "VariantAttributeValues" ADD "TenantId" character varying(100);

ALTER TABLE "VariantAddOns" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "VariantAddOns" ADD "TenantId" character varying(100);

ALTER TABLE "UnitOfMeasures" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "UnitOfMeasures" ADD "TenantId" character varying(100);

ALTER TABLE "StockDetails" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "StockDetails" ADD "TenantId" character varying(100);

ALTER TABLE "SerialItems" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "SerialItems" ADD "TenantId" character varying(100);

ALTER TABLE "SellerVariantPrices" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "SellerVariantPrices" ADD "TenantId" character varying(100);

ALTER TABLE "Sellers" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Sellers" ADD "TenantId" character varying(100);

ALTER TABLE "ReturnRequestTransitions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReturnRequestTransitions" ADD "TenantId" character varying(100);

ALTER TABLE "ReturnRequests" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReturnRequests" ADD "TenantId" character varying(100);

ALTER TABLE "ReturnLineSerials" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReturnLineSerials" ADD "TenantId" character varying(100);

ALTER TABLE "ReturnLines" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReturnLines" ADD "TenantId" character varying(100);

ALTER TABLE "ReservationTransitions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReservationTransitions" ADD "TenantId" character varying(100);

ALTER TABLE "ReservationAllocations" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReservationAllocations" ADD "TenantId" character varying(100);

ALTER TABLE "QualityStatuses" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "QualityStatuses" ADD "TenantId" character varying(100);

ALTER TABLE "ProductVariants" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ProductVariants" ADD "TenantId" character varying(100);

ALTER TABLE "Products" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Products" ADD "TenantId" character varying(100);

ALTER TABLE "ProductAttributeValues" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ProductAttributeValues" ADD "TenantId" character varying(100);

ALTER TABLE "PriceTypes" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "PriceTypes" ADD "TenantId" character varying(100);

ALTER TABLE "PriceChannels" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "PriceChannels" ADD "TenantId" character varying(100);

ALTER TABLE "Offers" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Offers" ADD "TenantId" character varying(100);

ALTER TABLE "Locations" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Locations" ADD "TenantId" character varying(100);

ALTER TABLE "InventoryTransactions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryTransactions" ADD "TenantId" character varying(100);

ALTER TABLE "InventoryTransactionLines" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryTransactionLines" ADD "TenantId" character varying(100);

ALTER TABLE "InventorySourceConsumptions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventorySourceConsumptions" ADD "TenantId" character varying(100);

ALTER TABLE "InventorySourceBalances" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventorySourceBalances" ADD "TenantId" character varying(100);

ALTER TABLE "InventorySourceAllocations" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventorySourceAllocations" ADD "TenantId" character varying(100);

ALTER TABLE "InventoryReservations" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryReservations" ADD "TenantId" character varying(100);

ALTER TABLE "InventoryDocuments" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryDocuments" ADD "TenantId" character varying(100);

ALTER TABLE "InventoryDocumentLineSerials" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryDocumentLineSerials" ADD "TenantId" character varying(100);

ALTER TABLE "InventoryDocumentLines" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryDocumentLines" ADD "TenantId" character varying(100);

ALTER TABLE "FulfillmentTransitions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "FulfillmentTransitions" ADD "TenantId" character varying(100);

ALTER TABLE "Fulfillments" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Fulfillments" ADD "TenantId" character varying(100);

ALTER TABLE "FulfillmentLineSerials" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "FulfillmentLineSerials" ADD "TenantId" character varying(100);

ALTER TABLE "FulfillmentLines" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "FulfillmentLines" ADD "TenantId" character varying(100);

ALTER TABLE "CategoryVariantNameFormulas" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "CategoryVariantNameFormulas" ADD "TenantId" character varying(100);

ALTER TABLE "CategoryVariantNameFormulaParts" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "CategoryVariantNameFormulaParts" ADD "TenantId" character varying(100);

ALTER TABLE "CategorySchemaVersions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "CategorySchemaVersions" ADD "TenantId" character varying(100);

ALTER TABLE "CategoryAttributeRules" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "CategoryAttributeRules" ADD "TenantId" character varying(100);

ALTER TABLE "Categories" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Categories" ADD "TenantId" character varying(100);

ALTER TABLE "AttributeOptions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "AttributeOptions" ADD "TenantId" character varying(100);

ALTER TABLE "AttributeDefinitions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "AttributeDefinitions" ADD "TenantId" character varying(100);

CREATE INDEX "IX_Warehouses_OrganizationBusinessKey" ON "Warehouses" ("OrganizationBusinessKey");

CREATE INDEX "IX_Warehouses_TenantId" ON "Warehouses" ("TenantId");

CREATE INDEX "IX_VariantUomConversions_OrganizationBusinessKey" ON "VariantUomConversions" ("OrganizationBusinessKey");

CREATE INDEX "IX_VariantUomConversions_TenantId" ON "VariantUomConversions" ("TenantId");

CREATE INDEX "IX_VariantImages_OrganizationBusinessKey" ON "VariantImages" ("OrganizationBusinessKey");

CREATE INDEX "IX_VariantImages_TenantId" ON "VariantImages" ("TenantId");

CREATE INDEX "IX_VariantComponents_OrganizationBusinessKey" ON "VariantComponents" ("OrganizationBusinessKey");

CREATE INDEX "IX_VariantComponents_TenantId" ON "VariantComponents" ("TenantId");

CREATE INDEX "IX_VariantAttributeValues_OrganizationBusinessKey" ON "VariantAttributeValues" ("OrganizationBusinessKey");

CREATE INDEX "IX_VariantAttributeValues_TenantId" ON "VariantAttributeValues" ("TenantId");

CREATE INDEX "IX_VariantAddOns_OrganizationBusinessKey" ON "VariantAddOns" ("OrganizationBusinessKey");

CREATE INDEX "IX_VariantAddOns_TenantId" ON "VariantAddOns" ("TenantId");

CREATE INDEX "IX_UnitOfMeasures_OrganizationBusinessKey" ON "UnitOfMeasures" ("OrganizationBusinessKey");

CREATE INDEX "IX_UnitOfMeasures_TenantId" ON "UnitOfMeasures" ("TenantId");

CREATE INDEX "IX_StockDetails_OrganizationBusinessKey" ON "StockDetails" ("OrganizationBusinessKey");

CREATE INDEX "IX_StockDetails_TenantId" ON "StockDetails" ("TenantId");

CREATE INDEX "IX_SerialItems_OrganizationBusinessKey" ON "SerialItems" ("OrganizationBusinessKey");

CREATE INDEX "IX_SerialItems_TenantId" ON "SerialItems" ("TenantId");

CREATE INDEX "IX_SellerVariantPrices_OrganizationBusinessKey" ON "SellerVariantPrices" ("OrganizationBusinessKey");

CREATE INDEX "IX_SellerVariantPrices_TenantId" ON "SellerVariantPrices" ("TenantId");

CREATE INDEX "IX_Sellers_OrganizationBusinessKey" ON "Sellers" ("OrganizationBusinessKey");

CREATE INDEX "IX_Sellers_TenantId" ON "Sellers" ("TenantId");

CREATE INDEX "IX_ReturnRequestTransitions_OrganizationBusinessKey" ON "ReturnRequestTransitions" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReturnRequestTransitions_TenantId" ON "ReturnRequestTransitions" ("TenantId");

CREATE INDEX "IX_ReturnRequests_OrganizationBusinessKey" ON "ReturnRequests" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReturnRequests_TenantId" ON "ReturnRequests" ("TenantId");

CREATE INDEX "IX_ReturnLineSerials_OrganizationBusinessKey" ON "ReturnLineSerials" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReturnLineSerials_TenantId" ON "ReturnLineSerials" ("TenantId");

CREATE INDEX "IX_ReturnLines_OrganizationBusinessKey" ON "ReturnLines" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReturnLines_TenantId" ON "ReturnLines" ("TenantId");

CREATE INDEX "IX_ReservationTransitions_OrganizationBusinessKey" ON "ReservationTransitions" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReservationTransitions_TenantId" ON "ReservationTransitions" ("TenantId");

CREATE INDEX "IX_ReservationAllocations_OrganizationBusinessKey" ON "ReservationAllocations" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReservationAllocations_TenantId" ON "ReservationAllocations" ("TenantId");

CREATE INDEX "IX_QualityStatuses_OrganizationBusinessKey" ON "QualityStatuses" ("OrganizationBusinessKey");

CREATE INDEX "IX_QualityStatuses_TenantId" ON "QualityStatuses" ("TenantId");

CREATE INDEX "IX_ProductVariants_OrganizationBusinessKey" ON "ProductVariants" ("OrganizationBusinessKey");

CREATE INDEX "IX_ProductVariants_TenantId" ON "ProductVariants" ("TenantId");

CREATE INDEX "IX_Products_OrganizationBusinessKey" ON "Products" ("OrganizationBusinessKey");

CREATE INDEX "IX_Products_TenantId" ON "Products" ("TenantId");

CREATE INDEX "IX_ProductAttributeValues_OrganizationBusinessKey" ON "ProductAttributeValues" ("OrganizationBusinessKey");

CREATE INDEX "IX_ProductAttributeValues_TenantId" ON "ProductAttributeValues" ("TenantId");

CREATE INDEX "IX_PriceTypes_OrganizationBusinessKey" ON "PriceTypes" ("OrganizationBusinessKey");

CREATE INDEX "IX_PriceTypes_TenantId" ON "PriceTypes" ("TenantId");

CREATE INDEX "IX_PriceChannels_OrganizationBusinessKey" ON "PriceChannels" ("OrganizationBusinessKey");

CREATE INDEX "IX_PriceChannels_TenantId" ON "PriceChannels" ("TenantId");

CREATE INDEX "IX_Offers_OrganizationBusinessKey" ON "Offers" ("OrganizationBusinessKey");

CREATE INDEX "IX_Offers_TenantId" ON "Offers" ("TenantId");

CREATE INDEX "IX_Locations_OrganizationBusinessKey" ON "Locations" ("OrganizationBusinessKey");

CREATE INDEX "IX_Locations_TenantId" ON "Locations" ("TenantId");

CREATE INDEX "IX_InventoryTransactions_OrganizationBusinessKey" ON "InventoryTransactions" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryTransactions_TenantId" ON "InventoryTransactions" ("TenantId");

CREATE INDEX "IX_InventoryTransactionLines_OrganizationBusinessKey" ON "InventoryTransactionLines" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryTransactionLines_TenantId" ON "InventoryTransactionLines" ("TenantId");

CREATE INDEX "IX_InventorySourceConsumptions_OrganizationBusinessKey" ON "InventorySourceConsumptions" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventorySourceConsumptions_TenantId" ON "InventorySourceConsumptions" ("TenantId");

CREATE INDEX "IX_InventorySourceBalances_OrganizationBusinessKey" ON "InventorySourceBalances" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventorySourceBalances_TenantId" ON "InventorySourceBalances" ("TenantId");

CREATE INDEX "IX_InventorySourceAllocations_OrganizationBusinessKey" ON "InventorySourceAllocations" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventorySourceAllocations_TenantId" ON "InventorySourceAllocations" ("TenantId");

CREATE INDEX "IX_InventoryReservations_OrganizationBusinessKey" ON "InventoryReservations" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryReservations_TenantId" ON "InventoryReservations" ("TenantId");

CREATE INDEX "IX_InventoryDocuments_OrganizationBusinessKey" ON "InventoryDocuments" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryDocuments_TenantId" ON "InventoryDocuments" ("TenantId");

CREATE INDEX "IX_InventoryDocumentLineSerials_OrganizationBusinessKey" ON "InventoryDocumentLineSerials" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryDocumentLineSerials_TenantId" ON "InventoryDocumentLineSerials" ("TenantId");

CREATE INDEX "IX_InventoryDocumentLines_OrganizationBusinessKey" ON "InventoryDocumentLines" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryDocumentLines_TenantId" ON "InventoryDocumentLines" ("TenantId");

CREATE INDEX "IX_FulfillmentTransitions_OrganizationBusinessKey" ON "FulfillmentTransitions" ("OrganizationBusinessKey");

CREATE INDEX "IX_FulfillmentTransitions_TenantId" ON "FulfillmentTransitions" ("TenantId");

CREATE INDEX "IX_Fulfillments_OrganizationBusinessKey" ON "Fulfillments" ("OrganizationBusinessKey");

CREATE INDEX "IX_Fulfillments_TenantId" ON "Fulfillments" ("TenantId");

CREATE INDEX "IX_FulfillmentLineSerials_OrganizationBusinessKey" ON "FulfillmentLineSerials" ("OrganizationBusinessKey");

CREATE INDEX "IX_FulfillmentLineSerials_TenantId" ON "FulfillmentLineSerials" ("TenantId");

CREATE INDEX "IX_FulfillmentLines_OrganizationBusinessKey" ON "FulfillmentLines" ("OrganizationBusinessKey");

CREATE INDEX "IX_FulfillmentLines_TenantId" ON "FulfillmentLines" ("TenantId");

CREATE INDEX "IX_CategoryVariantNameFormulas_OrganizationBusinessKey" ON "CategoryVariantNameFormulas" ("OrganizationBusinessKey");

CREATE INDEX "IX_CategoryVariantNameFormulas_TenantId" ON "CategoryVariantNameFormulas" ("TenantId");

CREATE INDEX "IX_CategoryVariantNameFormulaParts_OrganizationBusinessKey" ON "CategoryVariantNameFormulaParts" ("OrganizationBusinessKey");

CREATE INDEX "IX_CategoryVariantNameFormulaParts_TenantId" ON "CategoryVariantNameFormulaParts" ("TenantId");

CREATE INDEX "IX_CategorySchemaVersions_OrganizationBusinessKey" ON "CategorySchemaVersions" ("OrganizationBusinessKey");

CREATE INDEX "IX_CategorySchemaVersions_TenantId" ON "CategorySchemaVersions" ("TenantId");

CREATE INDEX "IX_CategoryAttributeRules_OrganizationBusinessKey" ON "CategoryAttributeRules" ("OrganizationBusinessKey");

CREATE INDEX "IX_CategoryAttributeRules_TenantId" ON "CategoryAttributeRules" ("TenantId");

CREATE INDEX "IX_Categories_OrganizationBusinessKey" ON "Categories" ("OrganizationBusinessKey");

CREATE INDEX "IX_Categories_TenantId" ON "Categories" ("TenantId");

CREATE INDEX "IX_AttributeOptions_OrganizationBusinessKey" ON "AttributeOptions" ("OrganizationBusinessKey");

CREATE INDEX "IX_AttributeOptions_TenantId" ON "AttributeOptions" ("TenantId");

CREATE INDEX "IX_AttributeDefinitions_OrganizationBusinessKey" ON "AttributeDefinitions" ("OrganizationBusinessKey");

CREATE INDEX "IX_AttributeDefinitions_TenantId" ON "AttributeDefinitions" ("TenantId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260513054336_AddTenantOrganizationScope', '9.0.11');

COMMIT;


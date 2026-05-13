START TRANSACTION;
ALTER TABLE "Warehouses" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "VariantUomConversions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "VariantImages" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "VariantComponents" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "VariantAttributeValues" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "VariantAddOns" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "UnitOfMeasures" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "StockDetails" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "SerialItems" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "SellerVariantPrices" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Sellers" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReturnRequestTransitions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReturnRequests" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReturnLineSerials" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReturnLines" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReservationTransitions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ReservationAllocations" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "QualityStatuses" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ProductVariants" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Products" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "ProductAttributeValues" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "PriceTypes" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "PriceChannels" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Offers" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Locations" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryTransactions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryTransactionLines" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventorySourceConsumptions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventorySourceBalances" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventorySourceAllocations" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryReservations" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryDocuments" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryDocumentLineSerials" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "InventoryDocumentLines" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "FulfillmentTransitions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Fulfillments" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "FulfillmentLineSerials" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "FulfillmentLines" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "CategoryVariantNameFormulas" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "CategoryVariantNameFormulaParts" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "CategorySchemaVersions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "CategoryAttributeRules" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "Categories" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "AttributeOptions" ADD "OrganizationBusinessKey" character varying(50);

ALTER TABLE "AttributeDefinitions" ADD "OrganizationBusinessKey" character varying(50);

CREATE INDEX "IX_Warehouses_OrganizationBusinessKey" ON "Warehouses" ("OrganizationBusinessKey");

CREATE INDEX "IX_VariantUomConversions_OrganizationBusinessKey" ON "VariantUomConversions" ("OrganizationBusinessKey");

CREATE INDEX "IX_VariantImages_OrganizationBusinessKey" ON "VariantImages" ("OrganizationBusinessKey");

CREATE INDEX "IX_VariantComponents_OrganizationBusinessKey" ON "VariantComponents" ("OrganizationBusinessKey");

CREATE INDEX "IX_VariantAttributeValues_OrganizationBusinessKey" ON "VariantAttributeValues" ("OrganizationBusinessKey");

CREATE INDEX "IX_VariantAddOns_OrganizationBusinessKey" ON "VariantAddOns" ("OrganizationBusinessKey");

CREATE INDEX "IX_UnitOfMeasures_OrganizationBusinessKey" ON "UnitOfMeasures" ("OrganizationBusinessKey");

CREATE INDEX "IX_StockDetails_OrganizationBusinessKey" ON "StockDetails" ("OrganizationBusinessKey");

CREATE INDEX "IX_SerialItems_OrganizationBusinessKey" ON "SerialItems" ("OrganizationBusinessKey");

CREATE INDEX "IX_SellerVariantPrices_OrganizationBusinessKey" ON "SellerVariantPrices" ("OrganizationBusinessKey");

CREATE INDEX "IX_Sellers_OrganizationBusinessKey" ON "Sellers" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReturnRequestTransitions_OrganizationBusinessKey" ON "ReturnRequestTransitions" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReturnRequests_OrganizationBusinessKey" ON "ReturnRequests" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReturnLineSerials_OrganizationBusinessKey" ON "ReturnLineSerials" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReturnLines_OrganizationBusinessKey" ON "ReturnLines" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReservationTransitions_OrganizationBusinessKey" ON "ReservationTransitions" ("OrganizationBusinessKey");

CREATE INDEX "IX_ReservationAllocations_OrganizationBusinessKey" ON "ReservationAllocations" ("OrganizationBusinessKey");

CREATE INDEX "IX_QualityStatuses_OrganizationBusinessKey" ON "QualityStatuses" ("OrganizationBusinessKey");

CREATE INDEX "IX_ProductVariants_OrganizationBusinessKey" ON "ProductVariants" ("OrganizationBusinessKey");

CREATE INDEX "IX_Products_OrganizationBusinessKey" ON "Products" ("OrganizationBusinessKey");

CREATE INDEX "IX_ProductAttributeValues_OrganizationBusinessKey" ON "ProductAttributeValues" ("OrganizationBusinessKey");

CREATE INDEX "IX_PriceTypes_OrganizationBusinessKey" ON "PriceTypes" ("OrganizationBusinessKey");

CREATE INDEX "IX_PriceChannels_OrganizationBusinessKey" ON "PriceChannels" ("OrganizationBusinessKey");

CREATE INDEX "IX_Offers_OrganizationBusinessKey" ON "Offers" ("OrganizationBusinessKey");

CREATE INDEX "IX_Locations_OrganizationBusinessKey" ON "Locations" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryTransactions_OrganizationBusinessKey" ON "InventoryTransactions" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryTransactionLines_OrganizationBusinessKey" ON "InventoryTransactionLines" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventorySourceConsumptions_OrganizationBusinessKey" ON "InventorySourceConsumptions" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventorySourceBalances_OrganizationBusinessKey" ON "InventorySourceBalances" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventorySourceAllocations_OrganizationBusinessKey" ON "InventorySourceAllocations" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryReservations_OrganizationBusinessKey" ON "InventoryReservations" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryDocuments_OrganizationBusinessKey" ON "InventoryDocuments" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryDocumentLineSerials_OrganizationBusinessKey" ON "InventoryDocumentLineSerials" ("OrganizationBusinessKey");

CREATE INDEX "IX_InventoryDocumentLines_OrganizationBusinessKey" ON "InventoryDocumentLines" ("OrganizationBusinessKey");

CREATE INDEX "IX_FulfillmentTransitions_OrganizationBusinessKey" ON "FulfillmentTransitions" ("OrganizationBusinessKey");

CREATE INDEX "IX_Fulfillments_OrganizationBusinessKey" ON "Fulfillments" ("OrganizationBusinessKey");

CREATE INDEX "IX_FulfillmentLineSerials_OrganizationBusinessKey" ON "FulfillmentLineSerials" ("OrganizationBusinessKey");

CREATE INDEX "IX_FulfillmentLines_OrganizationBusinessKey" ON "FulfillmentLines" ("OrganizationBusinessKey");

CREATE INDEX "IX_CategoryVariantNameFormulas_OrganizationBusinessKey" ON "CategoryVariantNameFormulas" ("OrganizationBusinessKey");

CREATE INDEX "IX_CategoryVariantNameFormulaParts_OrganizationBusinessKey" ON "CategoryVariantNameFormulaParts" ("OrganizationBusinessKey");

CREATE INDEX "IX_CategorySchemaVersions_OrganizationBusinessKey" ON "CategorySchemaVersions" ("OrganizationBusinessKey");

CREATE INDEX "IX_CategoryAttributeRules_OrganizationBusinessKey" ON "CategoryAttributeRules" ("OrganizationBusinessKey");

CREATE INDEX "IX_Categories_OrganizationBusinessKey" ON "Categories" ("OrganizationBusinessKey");

CREATE INDEX "IX_AttributeOptions_OrganizationBusinessKey" ON "AttributeOptions" ("OrganizationBusinessKey");

CREATE INDEX "IX_AttributeDefinitions_OrganizationBusinessKey" ON "AttributeDefinitions" ("OrganizationBusinessKey");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260513035806_AddOrganizationBusinessKey', '9.0.11');

COMMIT;


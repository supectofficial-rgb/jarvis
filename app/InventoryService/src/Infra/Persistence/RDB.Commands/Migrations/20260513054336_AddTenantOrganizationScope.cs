using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantOrganizationScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Warehouses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Warehouses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "VariantUomConversions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "VariantUomConversions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "VariantImages",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "VariantImages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "VariantComponents",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "VariantComponents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "VariantAttributeValues",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "VariantAttributeValues",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "VariantAddOns",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "VariantAddOns",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "UnitOfMeasures",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UnitOfMeasures",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "StockDetails",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "StockDetails",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "SerialItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "SerialItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "SellerVariantPrices",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "SellerVariantPrices",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Sellers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Sellers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReturnRequestTransitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ReturnRequestTransitions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReturnRequests",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ReturnRequests",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReturnLineSerials",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ReturnLineSerials",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReturnLines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ReturnLines",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReservationTransitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ReservationTransitions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReservationAllocations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ReservationAllocations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "QualityStatuses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "QualityStatuses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ProductVariants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ProductVariants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ProductAttributeValues",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ProductAttributeValues",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "PriceTypes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PriceTypes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "PriceChannels",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PriceChannels",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Offers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Offers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Locations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Locations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryTransactions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "InventoryTransactions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryTransactionLines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "InventoryTransactionLines",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventorySourceConsumptions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "InventorySourceConsumptions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventorySourceBalances",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "InventorySourceBalances",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventorySourceAllocations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "InventorySourceAllocations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryReservations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "InventoryReservations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryDocuments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "InventoryDocuments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryDocumentLineSerials",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "InventoryDocumentLineSerials",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryDocumentLines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "InventoryDocumentLines",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "FulfillmentTransitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "FulfillmentTransitions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Fulfillments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Fulfillments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "FulfillmentLineSerials",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "FulfillmentLineSerials",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "FulfillmentLines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "FulfillmentLines",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "CategoryVariantNameFormulas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CategoryVariantNameFormulas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "CategoryVariantNameFormulaParts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CategoryVariantNameFormulaParts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "CategorySchemaVersions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CategorySchemaVersions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "CategoryAttributeRules",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CategoryAttributeRules",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Categories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "AttributeOptions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "AttributeOptions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "AttributeDefinitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "AttributeDefinitions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_OrganizationBusinessKey",
                table: "Warehouses",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_TenantId",
                table: "Warehouses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantUomConversions_OrganizationBusinessKey",
                table: "VariantUomConversions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_VariantUomConversions_TenantId",
                table: "VariantUomConversions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantImages_OrganizationBusinessKey",
                table: "VariantImages",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_VariantImages_TenantId",
                table: "VariantImages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantComponents_OrganizationBusinessKey",
                table: "VariantComponents",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_VariantComponents_TenantId",
                table: "VariantComponents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantAttributeValues_OrganizationBusinessKey",
                table: "VariantAttributeValues",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_VariantAttributeValues_TenantId",
                table: "VariantAttributeValues",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantAddOns_OrganizationBusinessKey",
                table: "VariantAddOns",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_VariantAddOns_TenantId",
                table: "VariantAddOns",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_OrganizationBusinessKey",
                table: "UnitOfMeasures",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_TenantId",
                table: "UnitOfMeasures",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockDetails_OrganizationBusinessKey",
                table: "StockDetails",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_StockDetails_TenantId",
                table: "StockDetails",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SerialItems_OrganizationBusinessKey",
                table: "SerialItems",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_SerialItems_TenantId",
                table: "SerialItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerVariantPrices_OrganizationBusinessKey",
                table: "SellerVariantPrices",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_SellerVariantPrices_TenantId",
                table: "SellerVariantPrices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_OrganizationBusinessKey",
                table: "Sellers",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_TenantId",
                table: "Sellers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequestTransitions_OrganizationBusinessKey",
                table: "ReturnRequestTransitions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequestTransitions_TenantId",
                table: "ReturnRequestTransitions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_OrganizationBusinessKey",
                table: "ReturnRequests",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_TenantId",
                table: "ReturnRequests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLineSerials_OrganizationBusinessKey",
                table: "ReturnLineSerials",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLineSerials_TenantId",
                table: "ReturnLineSerials",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLines_OrganizationBusinessKey",
                table: "ReturnLines",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLines_TenantId",
                table: "ReturnLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTransitions_OrganizationBusinessKey",
                table: "ReservationTransitions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTransitions_TenantId",
                table: "ReservationTransitions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationAllocations_OrganizationBusinessKey",
                table: "ReservationAllocations",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationAllocations_TenantId",
                table: "ReservationAllocations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityStatuses_OrganizationBusinessKey",
                table: "QualityStatuses",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_QualityStatuses_TenantId",
                table: "QualityStatuses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_OrganizationBusinessKey",
                table: "ProductVariants",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_TenantId",
                table: "ProductVariants",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrganizationBusinessKey",
                table: "Products",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_OrganizationBusinessKey",
                table: "ProductAttributeValues",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_TenantId",
                table: "ProductAttributeValues",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceTypes_OrganizationBusinessKey",
                table: "PriceTypes",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_PriceTypes_TenantId",
                table: "PriceTypes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceChannels_OrganizationBusinessKey",
                table: "PriceChannels",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_PriceChannels_TenantId",
                table: "PriceChannels",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_OrganizationBusinessKey",
                table: "Offers",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_TenantId",
                table: "Offers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_OrganizationBusinessKey",
                table: "Locations",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_TenantId",
                table: "Locations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_OrganizationBusinessKey",
                table: "InventoryTransactions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_TenantId",
                table: "InventoryTransactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactionLines_OrganizationBusinessKey",
                table: "InventoryTransactionLines",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactionLines_TenantId",
                table: "InventoryTransactionLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySourceConsumptions_OrganizationBusinessKey",
                table: "InventorySourceConsumptions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySourceConsumptions_TenantId",
                table: "InventorySourceConsumptions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySourceBalances_OrganizationBusinessKey",
                table: "InventorySourceBalances",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySourceBalances_TenantId",
                table: "InventorySourceBalances",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySourceAllocations_OrganizationBusinessKey",
                table: "InventorySourceAllocations",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySourceAllocations_TenantId",
                table: "InventorySourceAllocations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_OrganizationBusinessKey",
                table: "InventoryReservations",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_TenantId",
                table: "InventoryReservations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocuments_OrganizationBusinessKey",
                table: "InventoryDocuments",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocuments_TenantId",
                table: "InventoryDocuments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocumentLineSerials_OrganizationBusinessKey",
                table: "InventoryDocumentLineSerials",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocumentLineSerials_TenantId",
                table: "InventoryDocumentLineSerials",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocumentLines_OrganizationBusinessKey",
                table: "InventoryDocumentLines",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocumentLines_TenantId",
                table: "InventoryDocumentLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentTransitions_OrganizationBusinessKey",
                table: "FulfillmentTransitions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentTransitions_TenantId",
                table: "FulfillmentTransitions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Fulfillments_OrganizationBusinessKey",
                table: "Fulfillments",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Fulfillments_TenantId",
                table: "Fulfillments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentLineSerials_OrganizationBusinessKey",
                table: "FulfillmentLineSerials",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentLineSerials_TenantId",
                table: "FulfillmentLineSerials",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentLines_OrganizationBusinessKey",
                table: "FulfillmentLines",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentLines_TenantId",
                table: "FulfillmentLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryVariantNameFormulas_OrganizationBusinessKey",
                table: "CategoryVariantNameFormulas",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryVariantNameFormulas_TenantId",
                table: "CategoryVariantNameFormulas",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryVariantNameFormulaParts_OrganizationBusinessKey",
                table: "CategoryVariantNameFormulaParts",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryVariantNameFormulaParts_TenantId",
                table: "CategoryVariantNameFormulaParts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySchemaVersions_OrganizationBusinessKey",
                table: "CategorySchemaVersions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySchemaVersions_TenantId",
                table: "CategorySchemaVersions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributeRules_OrganizationBusinessKey",
                table: "CategoryAttributeRules",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributeRules_TenantId",
                table: "CategoryAttributeRules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_OrganizationBusinessKey",
                table: "Categories",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId",
                table: "Categories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptions_OrganizationBusinessKey",
                table: "AttributeOptions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptions_TenantId",
                table: "AttributeOptions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_OrganizationBusinessKey",
                table: "AttributeDefinitions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_TenantId",
                table: "AttributeDefinitions",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warehouses_OrganizationBusinessKey",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_TenantId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_VariantUomConversions_OrganizationBusinessKey",
                table: "VariantUomConversions");

            migrationBuilder.DropIndex(
                name: "IX_VariantUomConversions_TenantId",
                table: "VariantUomConversions");

            migrationBuilder.DropIndex(
                name: "IX_VariantImages_OrganizationBusinessKey",
                table: "VariantImages");

            migrationBuilder.DropIndex(
                name: "IX_VariantImages_TenantId",
                table: "VariantImages");

            migrationBuilder.DropIndex(
                name: "IX_VariantComponents_OrganizationBusinessKey",
                table: "VariantComponents");

            migrationBuilder.DropIndex(
                name: "IX_VariantComponents_TenantId",
                table: "VariantComponents");

            migrationBuilder.DropIndex(
                name: "IX_VariantAttributeValues_OrganizationBusinessKey",
                table: "VariantAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_VariantAttributeValues_TenantId",
                table: "VariantAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_VariantAddOns_OrganizationBusinessKey",
                table: "VariantAddOns");

            migrationBuilder.DropIndex(
                name: "IX_VariantAddOns_TenantId",
                table: "VariantAddOns");

            migrationBuilder.DropIndex(
                name: "IX_UnitOfMeasures_OrganizationBusinessKey",
                table: "UnitOfMeasures");

            migrationBuilder.DropIndex(
                name: "IX_UnitOfMeasures_TenantId",
                table: "UnitOfMeasures");

            migrationBuilder.DropIndex(
                name: "IX_StockDetails_OrganizationBusinessKey",
                table: "StockDetails");

            migrationBuilder.DropIndex(
                name: "IX_StockDetails_TenantId",
                table: "StockDetails");

            migrationBuilder.DropIndex(
                name: "IX_SerialItems_OrganizationBusinessKey",
                table: "SerialItems");

            migrationBuilder.DropIndex(
                name: "IX_SerialItems_TenantId",
                table: "SerialItems");

            migrationBuilder.DropIndex(
                name: "IX_SellerVariantPrices_OrganizationBusinessKey",
                table: "SellerVariantPrices");

            migrationBuilder.DropIndex(
                name: "IX_SellerVariantPrices_TenantId",
                table: "SellerVariantPrices");

            migrationBuilder.DropIndex(
                name: "IX_Sellers_OrganizationBusinessKey",
                table: "Sellers");

            migrationBuilder.DropIndex(
                name: "IX_Sellers_TenantId",
                table: "Sellers");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequestTransitions_OrganizationBusinessKey",
                table: "ReturnRequestTransitions");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequestTransitions_TenantId",
                table: "ReturnRequestTransitions");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_OrganizationBusinessKey",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_TenantId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnLineSerials_OrganizationBusinessKey",
                table: "ReturnLineSerials");

            migrationBuilder.DropIndex(
                name: "IX_ReturnLineSerials_TenantId",
                table: "ReturnLineSerials");

            migrationBuilder.DropIndex(
                name: "IX_ReturnLines_OrganizationBusinessKey",
                table: "ReturnLines");

            migrationBuilder.DropIndex(
                name: "IX_ReturnLines_TenantId",
                table: "ReturnLines");

            migrationBuilder.DropIndex(
                name: "IX_ReservationTransitions_OrganizationBusinessKey",
                table: "ReservationTransitions");

            migrationBuilder.DropIndex(
                name: "IX_ReservationTransitions_TenantId",
                table: "ReservationTransitions");

            migrationBuilder.DropIndex(
                name: "IX_ReservationAllocations_OrganizationBusinessKey",
                table: "ReservationAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ReservationAllocations_TenantId",
                table: "ReservationAllocations");

            migrationBuilder.DropIndex(
                name: "IX_QualityStatuses_OrganizationBusinessKey",
                table: "QualityStatuses");

            migrationBuilder.DropIndex(
                name: "IX_QualityStatuses_TenantId",
                table: "QualityStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_OrganizationBusinessKey",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_TenantId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_Products_OrganizationBusinessKey",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValues_OrganizationBusinessKey",
                table: "ProductAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValues_TenantId",
                table: "ProductAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_PriceTypes_OrganizationBusinessKey",
                table: "PriceTypes");

            migrationBuilder.DropIndex(
                name: "IX_PriceTypes_TenantId",
                table: "PriceTypes");

            migrationBuilder.DropIndex(
                name: "IX_PriceChannels_OrganizationBusinessKey",
                table: "PriceChannels");

            migrationBuilder.DropIndex(
                name: "IX_PriceChannels_TenantId",
                table: "PriceChannels");

            migrationBuilder.DropIndex(
                name: "IX_Offers_OrganizationBusinessKey",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_TenantId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Locations_OrganizationBusinessKey",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Locations_TenantId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_OrganizationBusinessKey",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_TenantId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactionLines_OrganizationBusinessKey",
                table: "InventoryTransactionLines");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactionLines_TenantId",
                table: "InventoryTransactionLines");

            migrationBuilder.DropIndex(
                name: "IX_InventorySourceConsumptions_OrganizationBusinessKey",
                table: "InventorySourceConsumptions");

            migrationBuilder.DropIndex(
                name: "IX_InventorySourceConsumptions_TenantId",
                table: "InventorySourceConsumptions");

            migrationBuilder.DropIndex(
                name: "IX_InventorySourceBalances_OrganizationBusinessKey",
                table: "InventorySourceBalances");

            migrationBuilder.DropIndex(
                name: "IX_InventorySourceBalances_TenantId",
                table: "InventorySourceBalances");

            migrationBuilder.DropIndex(
                name: "IX_InventorySourceAllocations_OrganizationBusinessKey",
                table: "InventorySourceAllocations");

            migrationBuilder.DropIndex(
                name: "IX_InventorySourceAllocations_TenantId",
                table: "InventorySourceAllocations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservations_OrganizationBusinessKey",
                table: "InventoryReservations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservations_TenantId",
                table: "InventoryReservations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDocuments_OrganizationBusinessKey",
                table: "InventoryDocuments");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDocuments_TenantId",
                table: "InventoryDocuments");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDocumentLineSerials_OrganizationBusinessKey",
                table: "InventoryDocumentLineSerials");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDocumentLineSerials_TenantId",
                table: "InventoryDocumentLineSerials");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDocumentLines_OrganizationBusinessKey",
                table: "InventoryDocumentLines");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDocumentLines_TenantId",
                table: "InventoryDocumentLines");

            migrationBuilder.DropIndex(
                name: "IX_FulfillmentTransitions_OrganizationBusinessKey",
                table: "FulfillmentTransitions");

            migrationBuilder.DropIndex(
                name: "IX_FulfillmentTransitions_TenantId",
                table: "FulfillmentTransitions");

            migrationBuilder.DropIndex(
                name: "IX_Fulfillments_OrganizationBusinessKey",
                table: "Fulfillments");

            migrationBuilder.DropIndex(
                name: "IX_Fulfillments_TenantId",
                table: "Fulfillments");

            migrationBuilder.DropIndex(
                name: "IX_FulfillmentLineSerials_OrganizationBusinessKey",
                table: "FulfillmentLineSerials");

            migrationBuilder.DropIndex(
                name: "IX_FulfillmentLineSerials_TenantId",
                table: "FulfillmentLineSerials");

            migrationBuilder.DropIndex(
                name: "IX_FulfillmentLines_OrganizationBusinessKey",
                table: "FulfillmentLines");

            migrationBuilder.DropIndex(
                name: "IX_FulfillmentLines_TenantId",
                table: "FulfillmentLines");

            migrationBuilder.DropIndex(
                name: "IX_CategoryVariantNameFormulas_OrganizationBusinessKey",
                table: "CategoryVariantNameFormulas");

            migrationBuilder.DropIndex(
                name: "IX_CategoryVariantNameFormulas_TenantId",
                table: "CategoryVariantNameFormulas");

            migrationBuilder.DropIndex(
                name: "IX_CategoryVariantNameFormulaParts_OrganizationBusinessKey",
                table: "CategoryVariantNameFormulaParts");

            migrationBuilder.DropIndex(
                name: "IX_CategoryVariantNameFormulaParts_TenantId",
                table: "CategoryVariantNameFormulaParts");

            migrationBuilder.DropIndex(
                name: "IX_CategorySchemaVersions_OrganizationBusinessKey",
                table: "CategorySchemaVersions");

            migrationBuilder.DropIndex(
                name: "IX_CategorySchemaVersions_TenantId",
                table: "CategorySchemaVersions");

            migrationBuilder.DropIndex(
                name: "IX_CategoryAttributeRules_OrganizationBusinessKey",
                table: "CategoryAttributeRules");

            migrationBuilder.DropIndex(
                name: "IX_CategoryAttributeRules_TenantId",
                table: "CategoryAttributeRules");

            migrationBuilder.DropIndex(
                name: "IX_Categories_OrganizationBusinessKey",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_TenantId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_AttributeOptions_OrganizationBusinessKey",
                table: "AttributeOptions");

            migrationBuilder.DropIndex(
                name: "IX_AttributeOptions_TenantId",
                table: "AttributeOptions");

            migrationBuilder.DropIndex(
                name: "IX_AttributeDefinitions_OrganizationBusinessKey",
                table: "AttributeDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_AttributeDefinitions_TenantId",
                table: "AttributeDefinitions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "VariantUomConversions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "VariantUomConversions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "VariantImages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "VariantImages");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "VariantComponents");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "VariantComponents");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "VariantAttributeValues");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "VariantAttributeValues");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "VariantAddOns");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "VariantAddOns");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "UnitOfMeasures");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UnitOfMeasures");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "StockDetails");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "StockDetails");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "SerialItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SerialItems");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "SellerVariantPrices");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SellerVariantPrices");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReturnRequestTransitions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ReturnRequestTransitions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReturnLineSerials");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ReturnLineSerials");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReturnLines");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ReturnLines");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReservationTransitions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ReservationTransitions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReservationAllocations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ReservationAllocations");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "QualityStatuses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "QualityStatuses");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "PriceTypes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PriceTypes");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "PriceChannels");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PriceChannels");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryTransactionLines");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventoryTransactionLines");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventorySourceConsumptions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventorySourceConsumptions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventorySourceBalances");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventorySourceBalances");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventorySourceAllocations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventorySourceAllocations");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryReservations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventoryReservations");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryDocuments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventoryDocuments");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryDocumentLineSerials");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventoryDocumentLineSerials");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryDocumentLines");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InventoryDocumentLines");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "FulfillmentTransitions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FulfillmentTransitions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Fulfillments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Fulfillments");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "FulfillmentLineSerials");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FulfillmentLineSerials");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "FulfillmentLines");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FulfillmentLines");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "CategoryVariantNameFormulas");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CategoryVariantNameFormulas");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "CategoryVariantNameFormulaParts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CategoryVariantNameFormulaParts");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "CategorySchemaVersions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CategorySchemaVersions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "CategoryAttributeRules");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CategoryAttributeRules");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "AttributeOptions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AttributeOptions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "AttributeDefinitions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AttributeDefinitions");
        }
    }
}

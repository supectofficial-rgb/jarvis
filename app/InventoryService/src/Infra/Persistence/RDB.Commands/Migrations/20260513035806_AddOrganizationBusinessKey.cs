using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationBusinessKey : Migration
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
                name: "OrganizationBusinessKey",
                table: "VariantUomConversions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "VariantImages",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "VariantComponents",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "VariantAttributeValues",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "VariantAddOns",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "UnitOfMeasures",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "StockDetails",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "SerialItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "SellerVariantPrices",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Sellers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReturnRequestTransitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReturnRequests",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReturnLineSerials",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReturnLines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReservationTransitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ReservationAllocations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "QualityStatuses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ProductVariants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "ProductAttributeValues",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "PriceTypes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "PriceChannels",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Offers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Locations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryTransactions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryTransactionLines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventorySourceConsumptions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventorySourceBalances",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventorySourceAllocations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryReservations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryDocuments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryDocumentLineSerials",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "InventoryDocumentLines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "FulfillmentTransitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Fulfillments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "FulfillmentLineSerials",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "FulfillmentLines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "CategoryVariantNameFormulas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "CategoryVariantNameFormulaParts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "CategorySchemaVersions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "CategoryAttributeRules",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "Categories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "AttributeOptions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationBusinessKey",
                table: "AttributeDefinitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_OrganizationBusinessKey",
                table: "Warehouses",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_VariantUomConversions_OrganizationBusinessKey",
                table: "VariantUomConversions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_VariantImages_OrganizationBusinessKey",
                table: "VariantImages",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_VariantComponents_OrganizationBusinessKey",
                table: "VariantComponents",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_VariantAttributeValues_OrganizationBusinessKey",
                table: "VariantAttributeValues",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_VariantAddOns_OrganizationBusinessKey",
                table: "VariantAddOns",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_OrganizationBusinessKey",
                table: "UnitOfMeasures",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_StockDetails_OrganizationBusinessKey",
                table: "StockDetails",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_SerialItems_OrganizationBusinessKey",
                table: "SerialItems",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_SellerVariantPrices_OrganizationBusinessKey",
                table: "SellerVariantPrices",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_OrganizationBusinessKey",
                table: "Sellers",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequestTransitions_OrganizationBusinessKey",
                table: "ReturnRequestTransitions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_OrganizationBusinessKey",
                table: "ReturnRequests",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLineSerials_OrganizationBusinessKey",
                table: "ReturnLineSerials",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLines_OrganizationBusinessKey",
                table: "ReturnLines",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTransitions_OrganizationBusinessKey",
                table: "ReservationTransitions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationAllocations_OrganizationBusinessKey",
                table: "ReservationAllocations",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_QualityStatuses_OrganizationBusinessKey",
                table: "QualityStatuses",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_OrganizationBusinessKey",
                table: "ProductVariants",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrganizationBusinessKey",
                table: "Products",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_OrganizationBusinessKey",
                table: "ProductAttributeValues",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_PriceTypes_OrganizationBusinessKey",
                table: "PriceTypes",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_PriceChannels_OrganizationBusinessKey",
                table: "PriceChannels",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_OrganizationBusinessKey",
                table: "Offers",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_OrganizationBusinessKey",
                table: "Locations",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_OrganizationBusinessKey",
                table: "InventoryTransactions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactionLines_OrganizationBusinessKey",
                table: "InventoryTransactionLines",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySourceConsumptions_OrganizationBusinessKey",
                table: "InventorySourceConsumptions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySourceBalances_OrganizationBusinessKey",
                table: "InventorySourceBalances",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySourceAllocations_OrganizationBusinessKey",
                table: "InventorySourceAllocations",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservations_OrganizationBusinessKey",
                table: "InventoryReservations",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocuments_OrganizationBusinessKey",
                table: "InventoryDocuments",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocumentLineSerials_OrganizationBusinessKey",
                table: "InventoryDocumentLineSerials",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocumentLines_OrganizationBusinessKey",
                table: "InventoryDocumentLines",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentTransitions_OrganizationBusinessKey",
                table: "FulfillmentTransitions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Fulfillments_OrganizationBusinessKey",
                table: "Fulfillments",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentLineSerials_OrganizationBusinessKey",
                table: "FulfillmentLineSerials",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_FulfillmentLines_OrganizationBusinessKey",
                table: "FulfillmentLines",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryVariantNameFormulas_OrganizationBusinessKey",
                table: "CategoryVariantNameFormulas",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryVariantNameFormulaParts_OrganizationBusinessKey",
                table: "CategoryVariantNameFormulaParts",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySchemaVersions_OrganizationBusinessKey",
                table: "CategorySchemaVersions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributeRules_OrganizationBusinessKey",
                table: "CategoryAttributeRules",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_OrganizationBusinessKey",
                table: "Categories",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeOptions_OrganizationBusinessKey",
                table: "AttributeOptions",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_OrganizationBusinessKey",
                table: "AttributeDefinitions",
                column: "OrganizationBusinessKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warehouses_OrganizationBusinessKey",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_VariantUomConversions_OrganizationBusinessKey",
                table: "VariantUomConversions");

            migrationBuilder.DropIndex(
                name: "IX_VariantImages_OrganizationBusinessKey",
                table: "VariantImages");

            migrationBuilder.DropIndex(
                name: "IX_VariantComponents_OrganizationBusinessKey",
                table: "VariantComponents");

            migrationBuilder.DropIndex(
                name: "IX_VariantAttributeValues_OrganizationBusinessKey",
                table: "VariantAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_VariantAddOns_OrganizationBusinessKey",
                table: "VariantAddOns");

            migrationBuilder.DropIndex(
                name: "IX_UnitOfMeasures_OrganizationBusinessKey",
                table: "UnitOfMeasures");

            migrationBuilder.DropIndex(
                name: "IX_StockDetails_OrganizationBusinessKey",
                table: "StockDetails");

            migrationBuilder.DropIndex(
                name: "IX_SerialItems_OrganizationBusinessKey",
                table: "SerialItems");

            migrationBuilder.DropIndex(
                name: "IX_SellerVariantPrices_OrganizationBusinessKey",
                table: "SellerVariantPrices");

            migrationBuilder.DropIndex(
                name: "IX_Sellers_OrganizationBusinessKey",
                table: "Sellers");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequestTransitions_OrganizationBusinessKey",
                table: "ReturnRequestTransitions");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_OrganizationBusinessKey",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnLineSerials_OrganizationBusinessKey",
                table: "ReturnLineSerials");

            migrationBuilder.DropIndex(
                name: "IX_ReturnLines_OrganizationBusinessKey",
                table: "ReturnLines");

            migrationBuilder.DropIndex(
                name: "IX_ReservationTransitions_OrganizationBusinessKey",
                table: "ReservationTransitions");

            migrationBuilder.DropIndex(
                name: "IX_ReservationAllocations_OrganizationBusinessKey",
                table: "ReservationAllocations");

            migrationBuilder.DropIndex(
                name: "IX_QualityStatuses_OrganizationBusinessKey",
                table: "QualityStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_OrganizationBusinessKey",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_Products_OrganizationBusinessKey",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValues_OrganizationBusinessKey",
                table: "ProductAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_PriceTypes_OrganizationBusinessKey",
                table: "PriceTypes");

            migrationBuilder.DropIndex(
                name: "IX_PriceChannels_OrganizationBusinessKey",
                table: "PriceChannels");

            migrationBuilder.DropIndex(
                name: "IX_Offers_OrganizationBusinessKey",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Locations_OrganizationBusinessKey",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_OrganizationBusinessKey",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactionLines_OrganizationBusinessKey",
                table: "InventoryTransactionLines");

            migrationBuilder.DropIndex(
                name: "IX_InventorySourceConsumptions_OrganizationBusinessKey",
                table: "InventorySourceConsumptions");

            migrationBuilder.DropIndex(
                name: "IX_InventorySourceBalances_OrganizationBusinessKey",
                table: "InventorySourceBalances");

            migrationBuilder.DropIndex(
                name: "IX_InventorySourceAllocations_OrganizationBusinessKey",
                table: "InventorySourceAllocations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReservations_OrganizationBusinessKey",
                table: "InventoryReservations");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDocuments_OrganizationBusinessKey",
                table: "InventoryDocuments");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDocumentLineSerials_OrganizationBusinessKey",
                table: "InventoryDocumentLineSerials");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDocumentLines_OrganizationBusinessKey",
                table: "InventoryDocumentLines");

            migrationBuilder.DropIndex(
                name: "IX_FulfillmentTransitions_OrganizationBusinessKey",
                table: "FulfillmentTransitions");

            migrationBuilder.DropIndex(
                name: "IX_Fulfillments_OrganizationBusinessKey",
                table: "Fulfillments");

            migrationBuilder.DropIndex(
                name: "IX_FulfillmentLineSerials_OrganizationBusinessKey",
                table: "FulfillmentLineSerials");

            migrationBuilder.DropIndex(
                name: "IX_FulfillmentLines_OrganizationBusinessKey",
                table: "FulfillmentLines");

            migrationBuilder.DropIndex(
                name: "IX_CategoryVariantNameFormulas_OrganizationBusinessKey",
                table: "CategoryVariantNameFormulas");

            migrationBuilder.DropIndex(
                name: "IX_CategoryVariantNameFormulaParts_OrganizationBusinessKey",
                table: "CategoryVariantNameFormulaParts");

            migrationBuilder.DropIndex(
                name: "IX_CategorySchemaVersions_OrganizationBusinessKey",
                table: "CategorySchemaVersions");

            migrationBuilder.DropIndex(
                name: "IX_CategoryAttributeRules_OrganizationBusinessKey",
                table: "CategoryAttributeRules");

            migrationBuilder.DropIndex(
                name: "IX_Categories_OrganizationBusinessKey",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_AttributeOptions_OrganizationBusinessKey",
                table: "AttributeOptions");

            migrationBuilder.DropIndex(
                name: "IX_AttributeDefinitions_OrganizationBusinessKey",
                table: "AttributeDefinitions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "VariantUomConversions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "VariantImages");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "VariantComponents");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "VariantAttributeValues");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "VariantAddOns");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "UnitOfMeasures");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "StockDetails");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "SerialItems");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "SellerVariantPrices");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReturnRequestTransitions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReturnLineSerials");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReturnLines");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReservationTransitions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ReservationAllocations");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "QualityStatuses");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "PriceTypes");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "PriceChannels");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryTransactionLines");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventorySourceConsumptions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventorySourceBalances");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventorySourceAllocations");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryReservations");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryDocuments");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryDocumentLineSerials");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "InventoryDocumentLines");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "FulfillmentTransitions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Fulfillments");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "FulfillmentLineSerials");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "FulfillmentLines");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "CategoryVariantNameFormulas");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "CategoryVariantNameFormulaParts");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "CategorySchemaVersions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "CategoryAttributeRules");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "AttributeOptions");

            migrationBuilder.DropColumn(
                name: "OrganizationBusinessKey",
                table: "AttributeDefinitions");
        }
    }
}

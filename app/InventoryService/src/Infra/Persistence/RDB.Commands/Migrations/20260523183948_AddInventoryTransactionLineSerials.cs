using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryTransactionLineSerials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryTransactionLineSerials",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionLineRef = table.Column<Guid>(type: "uuid", nullable: false),
                    SerialRef = table.Column<Guid>(type: "uuid", nullable: true),
                    SerialNo = table.Column<string>(type: "text", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InventoryTransactionLineId = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrganizationBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransactionLineSerials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryTransactionLineSerials_InventoryTransactionLines_I~",
                        column: x => x.InventoryTransactionLineId,
                        principalTable: "InventoryTransactionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationStructureNodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WarehouseRef = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentStructureRef = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrganizationBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationStructureNodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationStructureSelections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocationRef = table.Column<Guid>(type: "uuid", nullable: false),
                    StructureRef = table.Column<Guid>(type: "uuid", nullable: false),
                    StructureValueRef = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrganizationBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationStructureSelections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationStructureValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StructureRef = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrganizationBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationStructureValues", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactionLineSerials_InventoryTransactionLineId",
                table: "InventoryTransactionLineSerials",
                column: "InventoryTransactionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactionLineSerials_OrganizationBusinessKey",
                table: "InventoryTransactionLineSerials",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactionLineSerials_TenantId",
                table: "InventoryTransactionLineSerials",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStructureNodes_OrganizationBusinessKey",
                table: "LocationStructureNodes",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStructureNodes_TenantId",
                table: "LocationStructureNodes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStructureNodes_WarehouseRef_Code",
                table: "LocationStructureNodes",
                columns: new[] { "WarehouseRef", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationStructureNodes_WarehouseRef_ParentStructureRef_Disp~",
                table: "LocationStructureNodes",
                columns: new[] { "WarehouseRef", "ParentStructureRef", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationStructureSelections_LocationRef_StructureRef",
                table: "LocationStructureSelections",
                columns: new[] { "LocationRef", "StructureRef" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationStructureSelections_OrganizationBusinessKey",
                table: "LocationStructureSelections",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStructureSelections_TenantId",
                table: "LocationStructureSelections",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStructureValues_OrganizationBusinessKey",
                table: "LocationStructureValues",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_LocationStructureValues_StructureRef_Code",
                table: "LocationStructureValues",
                columns: new[] { "StructureRef", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationStructureValues_StructureRef_DisplayOrder",
                table: "LocationStructureValues",
                columns: new[] { "StructureRef", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationStructureValues_TenantId",
                table: "LocationStructureValues",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryTransactionLineSerials");

            migrationBuilder.DropTable(
                name: "LocationStructureNodes");

            migrationBuilder.DropTable(
                name: "LocationStructureSelections");

            migrationBuilder.DropTable(
                name: "LocationStructureValues");
        }
    }
}

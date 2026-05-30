using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class DropVariantComponentWarehouseAndLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VariantComponents_VariantRef_ComponentVariantRef_WarehouseRef_LocationRef",
                table: "VariantComponents");

            migrationBuilder.DropColumn(
                name: "WarehouseRef",
                table: "VariantComponents");

            migrationBuilder.DropColumn(
                name: "LocationRef",
                table: "VariantComponents");

            migrationBuilder.CreateIndex(
                name: "IX_VariantComponents_VariantRef_ComponentVariantRef",
                table: "VariantComponents",
                columns: new[] { "VariantRef", "ComponentVariantRef" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VariantComponents_VariantRef_ComponentVariantRef",
                table: "VariantComponents");

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseRef",
                table: "VariantComponents",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<Guid>(
                name: "LocationRef",
                table: "VariantComponents",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_VariantComponents_VariantRef_ComponentVariantRef_WarehouseRef_LocationRef",
                table: "VariantComponents",
                columns: new[] { "VariantRef", "ComponentVariantRef", "WarehouseRef", "LocationRef" },
                unique: true);
        }
    }
}

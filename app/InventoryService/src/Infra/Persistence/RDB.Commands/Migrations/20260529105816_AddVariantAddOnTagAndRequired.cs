using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantAddOnTagAndRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VariantAddOns_VariantRef_AddOnVariantRef",
                table: "VariantAddOns");

            migrationBuilder.AlterColumn<Guid>(
                name: "AddOnVariantRef",
                table: "VariantAddOns",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "VariantAddOns",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "VariantAddOns",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VariantAddOns_VariantRef_AddOnVariantRef",
                table: "VariantAddOns",
                columns: new[] { "VariantRef", "AddOnVariantRef" },
                unique: true,
                filter: "\"AddOnVariantRef\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VariantAddOns_VariantRef_TagId",
                table: "VariantAddOns",
                columns: new[] { "VariantRef", "TagId" },
                unique: true,
                filter: "\"TagId\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_VariantAddOns_AddOnVariantRefOrTagId",
                table: "VariantAddOns",
                sql: "\"AddOnVariantRef\" IS NOT NULL OR \"TagId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VariantAddOns_VariantRef_AddOnVariantRef",
                table: "VariantAddOns");

            migrationBuilder.DropIndex(
                name: "IX_VariantAddOns_VariantRef_TagId",
                table: "VariantAddOns");

            migrationBuilder.DropCheckConstraint(
                name: "CK_VariantAddOns_AddOnVariantRefOrTagId",
                table: "VariantAddOns");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "VariantAddOns");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "VariantAddOns");

            migrationBuilder.AlterColumn<Guid>(
                name: "AddOnVariantRef",
                table: "VariantAddOns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VariantAddOns_VariantRef_AddOnVariantRef",
                table: "VariantAddOns",
                columns: new[] { "VariantRef", "AddOnVariantRef" },
                unique: true);
        }
    }
}

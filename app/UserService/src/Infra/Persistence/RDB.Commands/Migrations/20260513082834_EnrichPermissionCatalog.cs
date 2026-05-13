using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.UserService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class EnrichPermissionCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Permissions",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Module",
                table: "Permissions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Permissions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "Permissions",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)7);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Module",
                table: "Permissions",
                column: "Module");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Type",
                table: "Permissions",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Permissions_Code",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_Module",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_Type",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Module",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Permissions");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Permissions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentHeaderParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveredBy",
                table: "InventoryDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedBy",
                table: "InventoryDocuments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredBy",
                table: "InventoryDocuments");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "InventoryDocuments");
        }
    }
}

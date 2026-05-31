using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Migrations;

public partial class AddInventoryDocumentNoSequence : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            CREATE SEQUENCE IF NOT EXISTS "InventoryDocumentNoSequence"
                START WITH 1
                INCREMENT BY 1
                MINVALUE 1
                NO MAXVALUE
                CACHE 1;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""DROP SEQUENCE IF EXISTS "InventoryDocumentNoSequence";""");
    }
}

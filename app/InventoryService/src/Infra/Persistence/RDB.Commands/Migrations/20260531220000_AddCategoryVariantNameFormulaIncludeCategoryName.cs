using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Migrations;

public partial class AddCategoryVariantNameFormulaIncludeCategoryName : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IncludeCategoryName",
            table: "CategoryVariantNameFormulas",
            type: "boolean",
            nullable: false,
            defaultValue: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IncludeCategoryName",
            table: "CategoryVariantNameFormulas");
    }
}

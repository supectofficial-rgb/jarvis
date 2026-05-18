using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VariantTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VariantRef = table.Column<Guid>(type: "uuid", nullable: false),
                    TagName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TagColor = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrganizationBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProductVariantId = table.Column<long>(type: "bigint", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariantTags_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VariantTags_OrganizationBusinessKey",
                table: "VariantTags",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_VariantTags_ProductVariantId",
                table: "VariantTags",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantTags_TenantId",
                table: "VariantTags",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantTags_VariantRef_TagName",
                table: "VariantTags",
                columns: new[] { "VariantRef", "TagName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VariantTags");
        }
    }
}

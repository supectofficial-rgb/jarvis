using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsMasterAndVariantTagRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TagName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TagColor = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
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
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_BusinessKey",
                table: "Tags",
                column: "BusinessKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_OrganizationBusinessKey",
                table: "Tags",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagName",
                table: "Tags",
                column: "TagName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TenantId",
                table: "Tags",
                column: "TenantId");

            migrationBuilder.AddColumn<Guid>(
                name: "TagRef",
                table: "VariantTags",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql("""
                INSERT INTO "Tags" (
                    "TagName",
                    "TagColor",
                    "CreatedByUserId",
                    "CreatedDateTime",
                    "ModifiedByUserId",
                    "ModifiedDateTime",
                    "OrganizationBusinessKey",
                    "TenantId",
                    "BusinessKey"
                )
                SELECT DISTINCT ON (vt."TagName")
                    vt."TagName",
                    vt."TagColor",
                    NULL,
                    NULL,
                    NULL,
                    NULL,
                    vt."OrganizationBusinessKey",
                    vt."TenantId",
                    (
                        substr(vt.row_hash, 1, 8) || '-' ||
                        substr(vt.row_hash, 9, 4) || '-' ||
                        substr(vt.row_hash, 13, 4) || '-' ||
                        substr(vt.row_hash, 17, 4) || '-' ||
                        substr(vt.row_hash, 21, 12)
                    )::uuid
                FROM (
                    SELECT
                        vt.*,
                        md5(random()::text || clock_timestamp()::text) AS row_hash
                    FROM "VariantTags" vt
                ) vt
                ORDER BY vt."TagName", vt."TagColor" DESC NULLS LAST, vt."DisplayOrder", vt."Id";

                UPDATE "VariantTags" vt
                SET "TagRef" = t."BusinessKey"
                FROM "Tags" t
                WHERE vt."TagName" = t."TagName";
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "TagRef",
                table: "VariantTags",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VariantTags_TagRef",
                table: "VariantTags",
                column: "TagRef");

            migrationBuilder.CreateIndex(
                name: "IX_VariantTags_VariantRef_TagRef",
                table: "VariantTags",
                columns: new[] { "VariantRef", "TagRef" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VariantTags_Tags_TagRef",
                table: "VariantTags",
                column: "TagRef",
                principalTable: "Tags",
                principalColumn: "BusinessKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropIndex(
                name: "IX_VariantTags_VariantRef_TagName",
                table: "VariantTags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VariantTags_VariantRef_TagName",
                table: "VariantTags",
                columns: new[] { "VariantRef", "TagName" },
                unique: true);

            migrationBuilder.DropForeignKey(
                name: "FK_VariantTags_Tags_TagRef",
                table: "VariantTags");

            migrationBuilder.DropIndex(
                name: "IX_VariantTags_VariantRef_TagRef",
                table: "VariantTags");

            migrationBuilder.DropIndex(
                name: "IX_VariantTags_TagRef",
                table: "VariantTags");

            migrationBuilder.DropColumn(
                name: "TagRef",
                table: "VariantTags");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}

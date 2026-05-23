using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class FixQualityStatusOrganizationScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_name = 'QualityStatuses'
          AND column_name = 'OrganizationBusinessKey'
    ) THEN
        ALTER TABLE "QualityStatuses"
            ADD COLUMN "OrganizationBusinessKey" character varying(50);
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_name = 'QualityStatuses'
          AND column_name = 'TenantId'
    ) THEN
        ALTER TABLE "QualityStatuses"
            ADD COLUMN "TenantId" character varying(100);
    END IF;
END $$;
""");

            migrationBuilder.Sql("""
CREATE INDEX IF NOT EXISTS "IX_QualityStatuses_OrganizationBusinessKey"
    ON "QualityStatuses" ("OrganizationBusinessKey");
""");

            migrationBuilder.Sql("""
CREATE INDEX IF NOT EXISTS "IX_QualityStatuses_TenantId"
    ON "QualityStatuses" ("TenantId");
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
DROP INDEX IF EXISTS "IX_QualityStatuses_OrganizationBusinessKey";
""");

            migrationBuilder.Sql("""
DROP INDEX IF EXISTS "IX_QualityStatuses_TenantId";
""");

            migrationBuilder.Sql("""
ALTER TABLE "QualityStatuses"
    DROP COLUMN IF EXISTS "OrganizationBusinessKey";
""");

            migrationBuilder.Sql("""
ALTER TABLE "QualityStatuses"
    DROP COLUMN IF EXISTS "TenantId";
""");
        }
    }
}

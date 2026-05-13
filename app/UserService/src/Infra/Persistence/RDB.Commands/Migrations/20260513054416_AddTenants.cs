using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Insurance.UserService.Infra.Persistence.RDB.Commands.Migrations
{
    /// <inheritdoc />
    public partial class AddTenants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByOrganizationBusinessKey",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByPersonaBusinessKey",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByOrganizationBusinessKey",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByPersonaBusinessKey",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByOrganizationBusinessKey",
                table: "UserRoleAssignments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByPersonaBusinessKey",
                table: "UserRoleAssignments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByOrganizationBusinessKey",
                table: "UserRoleAssignments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByPersonaBusinessKey",
                table: "UserRoleAssignments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByOrganizationBusinessKey",
                table: "RolePermissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByPersonaBusinessKey",
                table: "RolePermissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByOrganizationBusinessKey",
                table: "RolePermissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByPersonaBusinessKey",
                table: "RolePermissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByOrganizationBusinessKey",
                table: "Permissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByPersonaBusinessKey",
                table: "Permissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByOrganizationBusinessKey",
                table: "Permissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByPersonaBusinessKey",
                table: "Permissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByOrganizationBusinessKey",
                table: "Organizations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByPersonaBusinessKey",
                table: "Organizations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByOrganizationBusinessKey",
                table: "Organizations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByPersonaBusinessKey",
                table: "Organizations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByOrganizationBusinessKey",
                table: "Memberships",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByPersonaBusinessKey",
                table: "Memberships",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByOrganizationBusinessKey",
                table: "Memberships",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByPersonaBusinessKey",
                table: "Memberships",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OtpCodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MobileNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    CreatedByOrganizationBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedByPersonaBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByOrganizationBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedByPersonaBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    OrganizationBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByOrganizationBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedByPersonaBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedByOrganizationBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedByPersonaBusinessKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtpCodes_CreatedAt",
                table: "OtpCodes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OtpCodes_MobileNumber",
                table: "OtpCodes",
                column: "MobileNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_OrganizationBusinessKey",
                table: "Tenants",
                column: "OrganizationBusinessKey");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_TenantId",
                table: "Tenants",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtpCodes");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropColumn(
                name: "CreatedByOrganizationBusinessKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedByPersonaBusinessKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ModifiedByOrganizationBusinessKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ModifiedByPersonaBusinessKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedByOrganizationBusinessKey",
                table: "UserRoleAssignments");

            migrationBuilder.DropColumn(
                name: "CreatedByPersonaBusinessKey",
                table: "UserRoleAssignments");

            migrationBuilder.DropColumn(
                name: "ModifiedByOrganizationBusinessKey",
                table: "UserRoleAssignments");

            migrationBuilder.DropColumn(
                name: "ModifiedByPersonaBusinessKey",
                table: "UserRoleAssignments");

            migrationBuilder.DropColumn(
                name: "CreatedByOrganizationBusinessKey",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedByPersonaBusinessKey",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedByOrganizationBusinessKey",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ModifiedByPersonaBusinessKey",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedByOrganizationBusinessKey",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CreatedByPersonaBusinessKey",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ModifiedByOrganizationBusinessKey",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ModifiedByPersonaBusinessKey",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CreatedByOrganizationBusinessKey",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CreatedByPersonaBusinessKey",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ModifiedByOrganizationBusinessKey",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ModifiedByPersonaBusinessKey",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CreatedByOrganizationBusinessKey",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "CreatedByPersonaBusinessKey",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "ModifiedByOrganizationBusinessKey",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "ModifiedByPersonaBusinessKey",
                table: "Memberships");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Insurance.Infra.Persistence.Sql.Commands.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LostClaimantInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LostBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    InsuranceCompanyBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    InsuranceCompanyBranch = table.Column<Guid>(type: "uuid", nullable: false),
                    Gender = table.Column<byte>(type: "smallint", nullable: false),
                    PolicyHolderFullName = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    MobileNumber = table.Column<string>(type: "text", nullable: true),
                    GroupHeadType = table.Column<byte>(type: "smallint", nullable: false),
                    CarSystemAndTipBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    VehiclePlateType = table.Column<byte>(type: "smallint", nullable: false),
                    PlateModelType = table.Column<byte>(type: "smallint", nullable: false),
                    VehiclePlateDesign = table.Column<int>(type: "integer", nullable: false),
                    VehiclePlate = table.Column<string>(type: "text", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LostClaimantInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Losts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LostClaimantInfoId = table.Column<long>(type: "bigint", nullable: false),
                    ProvinceBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    CityBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    AppraiserBusinessKey = table.Column<Guid>(type: "uuid", nullable: false),
                    AccidentLocation = table.Column<string>(type: "text", nullable: false),
                    AccidentDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LostNumber = table.Column<string>(type: "text", nullable: true),
                    PolicyHolder = table.Column<string>(type: "text", nullable: true),
                    PolicyNumber = table.Column<string>(type: "text", nullable: true),
                    ClaimType = table.Column<byte>(type: "smallint", nullable: true),
                    Branch = table.Column<string>(type: "text", nullable: true),
                    InspectionCity = table.Column<string>(type: "text", nullable: true),
                    InspectionAddress = table.Column<string>(type: "text", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SubmissionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedByUserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BusinessKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Losts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Losts_LostClaimantInfos_LostClaimantInfoId",
                        column: x => x.LostClaimantInfoId,
                        principalTable: "LostClaimantInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Losts_LostClaimantInfoId",
                table: "Losts",
                column: "LostClaimantInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Losts");

            migrationBuilder.DropTable(
                name: "LostClaimantInfos");
        }
    }
}

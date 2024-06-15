using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ODataCovid.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CountryRegion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    latitude = table.Column<float>(type: "real", nullable: true),
                    longitude = table.Column<float>(type: "real", nullable: true),
                    countryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryRegion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Actives",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    day = table.Column<DateTime>(type: "datetime2", nullable: true),
                    value = table.Column<float>(type: "real", nullable: true),
                    CountryRegionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actives", x => x.id);
                    table.ForeignKey(
                        name: "FK_Actives_CountryRegion_CountryRegionId",
                        column: x => x.CountryRegionId,
                        principalTable: "CountryRegion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Confirmeds",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    day = table.Column<DateTime>(type: "datetime2", nullable: true),
                    value = table.Column<float>(type: "real", nullable: true),
                    CountryRegionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Confirmeds", x => x.id);
                    table.ForeignKey(
                        name: "FK_Confirmeds_CountryRegion_CountryRegionId",
                        column: x => x.CountryRegionId,
                        principalTable: "CountryRegion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CovidDaily",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    day = table.Column<DateTime>(type: "datetime2", nullable: true),
                    personConfirmed = table.Column<long>(type: "bigint", nullable: true),
                    personDeath = table.Column<long>(type: "bigint", nullable: true),
                    personRecovered = table.Column<long>(type: "bigint", nullable: true),
                    personActive = table.Column<long>(type: "bigint", nullable: true),
                    countryregionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CovidDaily", x => x.id);
                    table.ForeignKey(
                        name: "FK_CovidDaily_CountryRegion_countryregionId",
                        column: x => x.countryregionId,
                        principalTable: "CountryRegion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deaths",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    day = table.Column<DateTime>(type: "datetime2", nullable: true),
                    value = table.Column<float>(type: "real", nullable: true),
                    CountryRegionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deaths", x => x.id);
                    table.ForeignKey(
                        name: "FK_Deaths_CountryRegion_CountryRegionId",
                        column: x => x.CountryRegionId,
                        principalTable: "CountryRegion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recovereds",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    day = table.Column<DateTime>(type: "datetime2", nullable: true),
                    value = table.Column<float>(type: "real", nullable: true),
                    CountryRegionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recovereds", x => x.id);
                    table.ForeignKey(
                        name: "FK_Recovereds_CountryRegion_CountryRegionId",
                        column: x => x.CountryRegionId,
                        principalTable: "CountryRegion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actives_CountryRegionId",
                table: "Actives",
                column: "CountryRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Confirmeds_CountryRegionId",
                table: "Confirmeds",
                column: "CountryRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_CovidDaily_countryregionId",
                table: "CovidDaily",
                column: "countryregionId");

            migrationBuilder.CreateIndex(
                name: "IX_Deaths_CountryRegionId",
                table: "Deaths",
                column: "CountryRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Recovereds_CountryRegionId",
                table: "Recovereds",
                column: "CountryRegionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actives");

            migrationBuilder.DropTable(
                name: "Confirmeds");

            migrationBuilder.DropTable(
                name: "CovidDaily");

            migrationBuilder.DropTable(
                name: "Deaths");

            migrationBuilder.DropTable(
                name: "Recovereds");

            migrationBuilder.DropTable(
                name: "CountryRegion");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoricalUsageEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Commodity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commodity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketDocumentReferenceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentReferenceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meter",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServicePoint",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePoint", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitOfMeasure",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasure", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketDocumentReference",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketDocumentItemId = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceTypeId = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentReference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketDocumentReference_MarketDocumentItem_MarketDocumentItemId",
                        column: x => x.MarketDocumentItemId,
                        principalTable: "MarketDocumentItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketDocumentReference_MarketDocumentReferenceType_ReferenceTypeId",
                        column: x => x.ReferenceTypeId,
                        principalTable: "MarketDocumentReferenceType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HistoricalUsage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    ServicePointId = table.Column<long>(type: "bigint", nullable: false),
                    MeterId = table.Column<long>(type: "bigint", nullable: true),
                    PeriodStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Consumption = table.Column<double>(type: "float", nullable: false),
                    CommodityId = table.Column<int>(type: "int", nullable: false),
                    UnitOfMeasureId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricalUsage_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistoricalUsage_Commodity_CommodityId",
                        column: x => x.CommodityId,
                        principalTable: "Commodity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistoricalUsage_Meter_MeterId",
                        column: x => x.MeterId,
                        principalTable: "Meter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistoricalUsage_ServicePoint_ServicePointId",
                        column: x => x.ServicePointId,
                        principalTable: "ServicePoint",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistoricalUsage_UnitOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasure",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Commodity",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Electric" });

            migrationBuilder.InsertData(
                table: "MarketDocumentReferenceType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "HistoricalUsage" },
                    { 2, "Usage" },
                    { 3, "Invoice" },
                    { 4, "ServiceStart" },
                    { 5, "ServiceEnd" },
                    { 6, "ServiceRequest" }
                });

            migrationBuilder.InsertData(
                table: "UnitOfMeasure",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "KilowattHour" });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalUsage_AccountId",
                table: "HistoricalUsage",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalUsage_CommodityId",
                table: "HistoricalUsage",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalUsage_MeterId",
                table: "HistoricalUsage",
                column: "MeterId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalUsage_ServicePointId",
                table: "HistoricalUsage",
                column: "ServicePointId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalUsage_UnitOfMeasureId",
                table: "HistoricalUsage",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentReference_MarketDocumentItemId",
                table: "MarketDocumentReference",
                column: "MarketDocumentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentReference_ReferenceTypeId",
                table: "MarketDocumentReference",
                column: "ReferenceTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricalUsage");

            migrationBuilder.DropTable(
                name: "MarketDocumentReference");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Commodity");

            migrationBuilder.DropTable(
                name: "Meter");

            migrationBuilder.DropTable(
                name: "ServicePoint");

            migrationBuilder.DropTable(
                name: "UnitOfMeasure");

            migrationBuilder.DropTable(
                name: "MarketDocumentReferenceType");
        }
    }
}

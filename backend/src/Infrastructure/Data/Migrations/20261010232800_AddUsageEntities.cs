using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsageEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsageStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsageType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsageTypeId = table.Column<int>(type: "int", nullable: false),
                    OriginalUsageId = table.Column<long>(type: "bigint", nullable: true),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    StartRead = table.Column<int>(type: "int", nullable: false),
                    EndRead = table.Column<int>(type: "int", nullable: false),
                    UsageStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    ServicePointId = table.Column<long>(type: "bigint", nullable: false),
                    MeterId = table.Column<long>(type: "bigint", nullable: true),
                    PeriodStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Consumption = table.Column<double>(type: "float", nullable: false),
                    CommodityId = table.Column<int>(type: "int", nullable: false),
                    UnitOfMeasureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usage_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Usage_Commodity_CommodityId",
                        column: x => x.CommodityId,
                        principalTable: "Commodity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Usage_Meter_MeterId",
                        column: x => x.MeterId,
                        principalTable: "Meter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Usage_ServicePoint_ServicePointId",
                        column: x => x.ServicePointId,
                        principalTable: "ServicePoint",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Usage_UnitOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasure",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Usage_UsageStatus_UsageStatusId",
                        column: x => x.UsageStatusId,
                        principalTable: "UsageStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Usage_UsageType_UsageTypeId",
                        column: x => x.UsageTypeId,
                        principalTable: "UsageType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Usage_Usage_OriginalUsageId",
                        column: x => x.OriginalUsageId,
                        principalTable: "Usage",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "UsageStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "New" });

            migrationBuilder.InsertData(
                table: "UsageType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Original" },
                    { 2, "Cancel" },
                    { 3, "Replace" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usage_AccountId",
                table: "Usage",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Usage_CommodityId",
                table: "Usage",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_Usage_MeterId",
                table: "Usage",
                column: "MeterId");

            migrationBuilder.CreateIndex(
                name: "IX_Usage_OriginalUsageId",
                table: "Usage",
                column: "OriginalUsageId");

            migrationBuilder.CreateIndex(
                name: "IX_Usage_ServicePointId",
                table: "Usage",
                column: "ServicePointId");

            migrationBuilder.CreateIndex(
                name: "IX_Usage_UnitOfMeasureId",
                table: "Usage",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Usage_UsageStatusId",
                table: "Usage",
                column: "UsageStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Usage_UsageTypeId",
                table: "Usage",
                column: "UsageTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usage");

            migrationBuilder.DropTable(
                name: "UsageStatus");

            migrationBuilder.DropTable(
                name: "UsageType");
        }
    }
}

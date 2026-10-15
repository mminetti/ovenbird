using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceChargeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "UnitOfMeasure",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "InvoiceChargeCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceChargeCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceChargeType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceChargeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceCharge",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceChargeCodeId = table.Column<int>(type: "int", nullable: false),
                    InvoiceChargeTypeId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    ChargeAmount = table.Column<double>(type: "float", nullable: false),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    UnitOfMeasureId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceCharge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceCharge_InvoiceChargeCode_InvoiceChargeCodeId",
                        column: x => x.InvoiceChargeCodeId,
                        principalTable: "InvoiceChargeCode",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceCharge_InvoiceChargeType_InvoiceChargeTypeId",
                        column: x => x.InvoiceChargeTypeId,
                        principalTable: "InvoiceChargeType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceCharge_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceCharge_UnitOfMeasure_UnitOfMeasureId",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasure",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "InvoiceChargeType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Rate" },
                    { 2, "Account" },
                    { 3, "B2B" }
                });

            migrationBuilder.UpdateData(
                table: "UnitOfMeasure",
                keyColumn: "Id",
                keyValue: 1,
                column: "Identifier",
                value: "KH");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCharge_InvoiceChargeCodeId",
                table: "InvoiceCharge",
                column: "InvoiceChargeCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCharge_InvoiceChargeTypeId",
                table: "InvoiceCharge",
                column: "InvoiceChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCharge_InvoiceId",
                table: "InvoiceCharge",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceCharge_UnitOfMeasureId",
                table: "InvoiceCharge",
                column: "UnitOfMeasureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceCharge");

            migrationBuilder.DropTable(
                name: "InvoiceChargeCode");

            migrationBuilder.DropTable(
                name: "InvoiceChargeType");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "UnitOfMeasure");
        }
    }
}

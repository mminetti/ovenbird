using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceAndInvoicePaymentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceDate",
                table: "ServiceStartItem");

            migrationBuilder.CreateTable(
                name: "InvoicePaymentStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePaymentStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePurpose",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePurpose", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePayment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UtilityId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TraceNumber = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InvoicePaymentStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoicePayment_InvoicePaymentStatus_InvoicePaymentStatusId",
                        column: x => x.InvoicePaymentStatusId,
                        principalTable: "InvoicePaymentStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoicePayment_Utility_UtilityId",
                        column: x => x.UtilityId,
                        principalTable: "Utility",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    ServicePointId = table.Column<long>(type: "bigint", nullable: false),
                    InvoicePurposeId = table.Column<int>(type: "int", nullable: false),
                    InvoiceTypeId = table.Column<int>(type: "int", nullable: false),
                    OriginalInvoiceId = table.Column<long>(type: "bigint", nullable: true),
                    PeriodStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    InvoiceStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoice_InvoicePurpose_InvoicePurposeId",
                        column: x => x.InvoicePurposeId,
                        principalTable: "InvoicePurpose",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoice_InvoiceStatus_InvoiceStatusId",
                        column: x => x.InvoiceStatusId,
                        principalTable: "InvoiceStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoice_InvoiceType_InvoiceTypeId",
                        column: x => x.InvoiceTypeId,
                        principalTable: "InvoiceType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoice_Invoice_OriginalInvoiceId",
                        column: x => x.OriginalInvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoice_ServicePoint_ServicePointId",
                        column: x => x.ServicePointId,
                        principalTable: "ServicePoint",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoice_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvoicePaymentInvoice",
                columns: table => new
                {
                    InvoicePaymentId = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePaymentInvoice", x => new { x.InvoicePaymentId, x.InvoiceId });
                    table.ForeignKey(
                        name: "FK_InvoicePaymentInvoice_InvoicePayment_InvoicePaymentId",
                        column: x => x.InvoicePaymentId,
                        principalTable: "InvoicePayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoicePaymentInvoice_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "InvoicePurpose",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Original" },
                    { 2, "Cancel" },
                    { 3, "Replace" }
                });

            migrationBuilder.InsertData(
                table: "InvoiceType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Month" },
                    { 2, "Final" },
                    { 3, "ChangeNotice" },
                    { 4, "BalanceDue" },
                    { 5, "SecondNotice" },
                    { 6, "Tampering" },
                    { 7, "Charges" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_InvoicePurposeId",
                table: "Invoice",
                column: "InvoicePurposeId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_InvoiceStatusId",
                table: "Invoice",
                column: "InvoiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_InvoiceTypeId",
                table: "Invoice",
                column: "InvoiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_OriginalInvoiceId",
                table: "Invoice",
                column: "OriginalInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ServiceId",
                table: "Invoice",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ServicePointId",
                table: "Invoice",
                column: "ServicePointId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayment_InvoicePaymentStatusId",
                table: "InvoicePayment",
                column: "InvoicePaymentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayment_UtilityId",
                table: "InvoicePayment",
                column: "UtilityId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePaymentInvoice_InvoiceId",
                table: "InvoicePaymentInvoice",
                column: "InvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoicePaymentInvoice");

            migrationBuilder.DropTable(
                name: "InvoicePayment");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "InvoicePaymentStatus");

            migrationBuilder.DropTable(
                name: "InvoicePurpose");

            migrationBuilder.DropTable(
                name: "InvoiceStatus");

            migrationBuilder.DropTable(
                name: "InvoiceType");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ServiceDate",
                table: "ServiceStartItem",
                type: "date",
                nullable: true);
        }
    }
}

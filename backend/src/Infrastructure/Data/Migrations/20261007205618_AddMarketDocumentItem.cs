using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketDocumentItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketDocumentItemStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentItemStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketDocumentItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketDocumentId = table.Column<long>(type: "bigint", nullable: true),
                    MarketDocumentItemStatusId = table.Column<int>(type: "int", nullable: false),
                    Set = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SubSet = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SubPurpose = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    OriginalTrackingNumber = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ServicePointIdentifier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Raw = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketDocumentItem_MarketDocumentItemStatus_MarketDocumentItemStatusId",
                        column: x => x.MarketDocumentItemStatusId,
                        principalTable: "MarketDocumentItemStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarketDocumentItem_MarketDocument_MarketDocumentId",
                        column: x => x.MarketDocumentId,
                        principalTable: "MarketDocument",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "MarketDocumentItemStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "New" },
                    { 2, "Done" },
                    { 3, "Error" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentItem_MarketDocumentId",
                table: "MarketDocumentItem",
                column: "MarketDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentItem_MarketDocumentItemStatusId",
                table: "MarketDocumentItem",
                column: "MarketDocumentItemStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketDocumentItem");

            migrationBuilder.DropTable(
                name: "MarketDocumentItemStatus");
        }
    }
}

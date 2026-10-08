using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketDocumentItemField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketDocumentItemField",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketDocumentItemId = table.Column<long>(type: "bigint", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FieldValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentItemField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketDocumentItemField_MarketDocumentItem_MarketDocumentItemId",
                        column: x => x.MarketDocumentItemId,
                        principalTable: "MarketDocumentItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentItemField_MarketDocumentItemId",
                table: "MarketDocumentItemField",
                column: "MarketDocumentItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketDocumentItemField");
        }
    }
}

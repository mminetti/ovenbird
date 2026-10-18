using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitMarketDocumentAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File",
                table: "MarketDocument");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MarketDocument");

            migrationBuilder.AddColumn<long>(
                name: "AssetId",
                table: "MarketDocument",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "MarketDocument",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Asset",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocument_AssetId",
                table: "MarketDocument",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketDocument_Asset_AssetId",
                table: "MarketDocument",
                column: "AssetId",
                principalTable: "Asset",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketDocument_Asset_AssetId",
                table: "MarketDocument");

            migrationBuilder.DropTable(
                name: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_MarketDocument_AssetId",
                table: "MarketDocument");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "MarketDocument");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "MarketDocument");

            migrationBuilder.AddColumn<string>(
                name: "File",
                table: "MarketDocument",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MarketDocument",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}

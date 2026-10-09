using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMarketDocumentItemFieldAndOriginalReferenceAndRenameReferenceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalTrackingNumber",
                table: "MarketDocumentItem");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "MarketDocumentItem");

            migrationBuilder.DropColumn(
                name: "SubPurpose",
                table: "MarketDocumentItem");

            migrationBuilder.RenameColumn(
                name: "TrackingNumber",
                table: "MarketDocumentItem",
                newName: "ReferenceNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReferenceNumber",
                table: "MarketDocumentItem",
                newName: "TrackingNumber");

            migrationBuilder.AddColumn<string>(
                name: "OriginalTrackingNumber",
                table: "MarketDocumentItem",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "MarketDocumentItem",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubPurpose",
                table: "MarketDocumentItem",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}

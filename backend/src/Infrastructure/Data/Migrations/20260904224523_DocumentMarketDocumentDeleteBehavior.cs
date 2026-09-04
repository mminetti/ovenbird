using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DocumentMarketDocumentDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketDocument_Company_CompanyId",
                table: "MarketDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketDocument_MarketDocumentDirection_DirectionId",
                table: "MarketDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketDocument_MarketDocumentStatus_StatusId",
                table: "MarketDocument");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketDocument_Company_CompanyId",
                table: "MarketDocument",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketDocument_MarketDocumentDirection_DirectionId",
                table: "MarketDocument",
                column: "DirectionId",
                principalTable: "MarketDocumentDirection",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketDocument_MarketDocumentStatus_StatusId",
                table: "MarketDocument",
                column: "StatusId",
                principalTable: "MarketDocumentStatus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketDocument_Company_CompanyId",
                table: "MarketDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketDocument_MarketDocumentDirection_DirectionId",
                table: "MarketDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketDocument_MarketDocumentStatus_StatusId",
                table: "MarketDocument");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketDocument_Company_CompanyId",
                table: "MarketDocument",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketDocument_MarketDocumentDirection_DirectionId",
                table: "MarketDocument",
                column: "DirectionId",
                principalTable: "MarketDocumentDirection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketDocument_MarketDocumentStatus_StatusId",
                table: "MarketDocument",
                column: "StatusId",
                principalTable: "MarketDocumentStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

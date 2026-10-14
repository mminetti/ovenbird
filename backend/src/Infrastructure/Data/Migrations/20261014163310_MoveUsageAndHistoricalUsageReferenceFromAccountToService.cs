using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveUsageAndHistoricalUsageReferenceFromAccountToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricalUsage_Account_AccountId",
                table: "HistoricalUsage");

            migrationBuilder.DropForeignKey(
                name: "FK_Usage_Account_AccountId",
                table: "Usage");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Usage",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Usage_AccountId",
                table: "Usage",
                newName: "IX_Usage_ServiceId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "HistoricalUsage",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_HistoricalUsage_AccountId",
                table: "HistoricalUsage",
                newName: "IX_HistoricalUsage_ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricalUsage_Service_ServiceId",
                table: "HistoricalUsage",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usage_Service_ServiceId",
                table: "Usage",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricalUsage_Service_ServiceId",
                table: "HistoricalUsage");

            migrationBuilder.DropForeignKey(
                name: "FK_Usage_Service_ServiceId",
                table: "Usage");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Usage",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Usage_ServiceId",
                table: "Usage",
                newName: "IX_Usage_AccountId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "HistoricalUsage",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_HistoricalUsage_ServiceId",
                table: "HistoricalUsage",
                newName: "IX_HistoricalUsage_AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricalUsage_Account_AccountId",
                table: "HistoricalUsage",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usage_Account_AccountId",
                table: "Usage",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id");
        }
    }
}

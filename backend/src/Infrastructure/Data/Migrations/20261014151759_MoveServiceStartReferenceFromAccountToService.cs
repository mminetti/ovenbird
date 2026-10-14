using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveServiceStartReferenceFromAccountToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceStart_Account_AccountId",
                table: "ServiceStart");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "ServiceStart",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceStart_AccountId",
                table: "ServiceStart",
                newName: "IX_ServiceStart_ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceStart_Service_ServiceId",
                table: "ServiceStart",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceStart_Service_ServiceId",
                table: "ServiceStart");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "ServiceStart",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceStart_ServiceId",
                table: "ServiceStart",
                newName: "IX_ServiceStart_AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceStart_Account_AccountId",
                table: "ServiceStart",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id");
        }
    }
}

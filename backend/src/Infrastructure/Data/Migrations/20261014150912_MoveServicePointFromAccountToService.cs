using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveServicePointFromAccountToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_ServicePoint_ServicePointId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ServicePointId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ServicePointId",
                table: "Account");

            migrationBuilder.AddColumn<long>(
                name: "ServicePointId",
                table: "Service",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Service_ServicePointId",
                table: "Service",
                column: "ServicePointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServicePoint_ServicePointId",
                table: "Service",
                column: "ServicePointId",
                principalTable: "ServicePoint",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServicePoint_ServicePointId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_ServicePointId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ServicePointId",
                table: "Service");

            migrationBuilder.AddColumn<long>(
                name: "ServicePointId",
                table: "Account",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Account_ServicePointId",
                table: "Account",
                column: "ServicePointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ServicePoint_ServicePointId",
                table: "Account",
                column: "ServicePointId",
                principalTable: "ServicePoint",
                principalColumn: "Id");
        }
    }
}

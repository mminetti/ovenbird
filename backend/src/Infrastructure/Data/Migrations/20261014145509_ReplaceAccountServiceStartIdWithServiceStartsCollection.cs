using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceAccountServiceStartIdWithServiceStartsCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_ServiceStart_ServiceStartId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ServiceStartId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ServiceStartId",
                table: "Account");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ServiceStartId",
                table: "Account",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Account_ServiceStartId",
                table: "Account",
                column: "ServiceStartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ServiceStart_ServiceStartId",
                table: "Account",
                column: "ServiceStartId",
                principalTable: "ServiceStart",
                principalColumn: "Id");
        }
    }
}

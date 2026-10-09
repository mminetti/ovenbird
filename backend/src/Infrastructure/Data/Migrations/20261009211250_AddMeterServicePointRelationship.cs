using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMeterServicePointRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Meter",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ServicePointId",
                table: "Meter",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Meter_ServicePointId",
                table: "Meter",
                column: "ServicePointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meter_ServicePoint_ServicePointId",
                table: "Meter",
                column: "ServicePointId",
                principalTable: "ServicePoint",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meter_ServicePoint_ServicePointId",
                table: "Meter");

            migrationBuilder.DropIndex(
                name: "IX_Meter_ServicePointId",
                table: "Meter");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Meter");

            migrationBuilder.DropColumn(
                name: "ServicePointId",
                table: "Meter");
        }
    }
}

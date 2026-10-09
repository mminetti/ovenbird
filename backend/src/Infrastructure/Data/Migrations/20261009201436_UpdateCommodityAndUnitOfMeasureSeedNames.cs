using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommodityAndUnitOfMeasureSeedNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Commodity",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Electricity");

            migrationBuilder.UpdateData(
                table: "UnitOfMeasure",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Kilowatt Hour");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Commodity",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Electric");

            migrationBuilder.UpdateData(
                table: "UnitOfMeasure",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "KilowattHour");
        }
    }
}

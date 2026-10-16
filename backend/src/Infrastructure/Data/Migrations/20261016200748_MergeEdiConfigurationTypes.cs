using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MergeEdiConfigurationTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConfigurationType",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "ConfigurationType",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "EDI");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConfigurationType",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "EDI Import");

            migrationBuilder.InsertData(
                table: "ConfigurationType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 3, null, "EDI Export" });
        }
    }
}

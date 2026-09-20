using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurationTypeConstants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConfigurationType",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "ConfigurationType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, null, "EDI Import" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConfigurationType",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "ConfigurationType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, null, "EDI Import" });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConnectorTypeSeedNameUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConnectorType",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "FTP");

            migrationBuilder.UpdateData(
                table: "ConnectorType",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "File Storage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ConnectorType",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "ftp");

            migrationBuilder.UpdateData(
                table: "ConnectorType",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "file.storage");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveConnectorTypeToConnectorImplementation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connector_ConnectorType_ConnectorTypeId",
                table: "Connector");

            migrationBuilder.DropIndex(
                name: "IX_Connector_ConnectorTypeId",
                table: "Connector");

            migrationBuilder.DropColumn(
                name: "ConnectorTypeId",
                table: "Connector");

            migrationBuilder.AddColumn<int>(
                name: "ConnectorTypeId",
                table: "ConnectorImplementation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "ConnectorImplementation",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ConnectorImplementation",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConnectorTypeId", "Identifier", "Name" },
                values: new object[] { 1, "FluentFtpService", "Fluent FTP" });

            migrationBuilder.UpdateData(
                table: "ConnectorImplementation",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConnectorTypeId", "Identifier", "Name" },
                values: new object[] { 1, "SshNetSftpService", "SSH.NET SFTP" });

            migrationBuilder.UpdateData(
                table: "ConnectorImplementation",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConnectorTypeId", "Identifier", "Name" },
                values: new object[] { 1, "LocalFileSystemFtpService", "Local File System FTP" });

            migrationBuilder.UpdateData(
                table: "ConnectorImplementation",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConnectorTypeId", "Identifier", "Name" },
                values: new object[] { 2, "AzureBlobFileStorage", "Azure Blob Storage" });

            migrationBuilder.UpdateData(
                table: "ConnectorImplementation",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConnectorTypeId", "Identifier", "Name" },
                values: new object[] { 2, "LocalFileSystemFileStorage", "Local File System Storage" });

            migrationBuilder.CreateIndex(
                name: "IX_ConnectorImplementation_ConnectorTypeId",
                table: "ConnectorImplementation",
                column: "ConnectorTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConnectorImplementation_ConnectorType_ConnectorTypeId",
                table: "ConnectorImplementation",
                column: "ConnectorTypeId",
                principalTable: "ConnectorType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConnectorImplementation_ConnectorType_ConnectorTypeId",
                table: "ConnectorImplementation");

            migrationBuilder.DropIndex(
                name: "IX_ConnectorImplementation_ConnectorTypeId",
                table: "ConnectorImplementation");

            migrationBuilder.DropColumn(
                name: "ConnectorTypeId",
                table: "ConnectorImplementation");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "ConnectorImplementation");

            migrationBuilder.AddColumn<int>(
                name: "ConnectorTypeId",
                table: "Connector",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "ConnectorImplementation",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "FluentFtpService");

            migrationBuilder.UpdateData(
                table: "ConnectorImplementation",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "SshNetSftpService");

            migrationBuilder.UpdateData(
                table: "ConnectorImplementation",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "LocalFileSystemFtpService");

            migrationBuilder.UpdateData(
                table: "ConnectorImplementation",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "AzureBlobFileStorage");

            migrationBuilder.UpdateData(
                table: "ConnectorImplementation",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "LocalFileSystemFileStorage");

            migrationBuilder.CreateIndex(
                name: "IX_Connector_ConnectorTypeId",
                table: "Connector",
                column: "ConnectorTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Connector_ConnectorType_ConnectorTypeId",
                table: "Connector",
                column: "ConnectorTypeId",
                principalTable: "ConnectorType",
                principalColumn: "Id");
        }
    }
}

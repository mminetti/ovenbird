using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidateErrorLogging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketDocumentEventItem_MarketDocumentEventItemLevel_LevelId",
                table: "MarketDocumentEventItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessRunLogItem_ProcessRunLogLevel_LevelId",
                table: "ProcessRunLogItem");

            migrationBuilder.DropTable(
                name: "MarketDocumentEventItemLevel");

            migrationBuilder.DropTable(
                name: "ProcessRunLogLevel");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "MarketDocumentEvent");

            migrationBuilder.RenameColumn(
                name: "LevelId",
                table: "ProcessRunLogItem",
                newName: "SeverityId");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessRunLogItem_LevelId",
                table: "ProcessRunLogItem",
                newName: "IX_ProcessRunLogItem_SeverityId");

            migrationBuilder.RenameColumn(
                name: "LevelId",
                table: "MarketDocumentEventItem",
                newName: "SeverityId");

            migrationBuilder.RenameIndex(
                name: "IX_MarketDocumentEventItem_LevelId",
                table: "MarketDocumentEventItem",
                newName: "IX_MarketDocumentEventItem_SeverityId");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ProcessRunLogItem",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedAtUtc",
                table: "ProcessRunLogItem",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "ProcessRunLogItem",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ErrorSeverity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorSeverity", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ErrorSeverity",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Error" },
                    { 2, "Warning" },
                    { 3, "Info" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_MarketDocumentEventItem_ErrorSeverity_SeverityId",
                table: "MarketDocumentEventItem",
                column: "SeverityId",
                principalTable: "ErrorSeverity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessRunLogItem_ErrorSeverity_SeverityId",
                table: "ProcessRunLogItem",
                column: "SeverityId",
                principalTable: "ErrorSeverity",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketDocumentEventItem_ErrorSeverity_SeverityId",
                table: "MarketDocumentEventItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessRunLogItem_ErrorSeverity_SeverityId",
                table: "ProcessRunLogItem");

            migrationBuilder.DropTable(
                name: "ErrorSeverity");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProcessRunLogItem");

            migrationBuilder.DropColumn(
                name: "LastModifiedAtUtc",
                table: "ProcessRunLogItem");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ProcessRunLogItem");

            migrationBuilder.RenameColumn(
                name: "SeverityId",
                table: "ProcessRunLogItem",
                newName: "LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessRunLogItem_SeverityId",
                table: "ProcessRunLogItem",
                newName: "IX_ProcessRunLogItem_LevelId");

            migrationBuilder.RenameColumn(
                name: "SeverityId",
                table: "MarketDocumentEventItem",
                newName: "LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_MarketDocumentEventItem_SeverityId",
                table: "MarketDocumentEventItem",
                newName: "IX_MarketDocumentEventItem_LevelId");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "MarketDocumentEvent",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MarketDocumentEventItemLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentEventItemLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessRunLogLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessRunLogLevel", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "MarketDocumentEventItemLevel",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Error" },
                    { 2, "Warning" },
                    { 3, "Info" }
                });

            migrationBuilder.InsertData(
                table: "ProcessRunLogLevel",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Error" },
                    { 2, "Warning" },
                    { 3, "Info" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_MarketDocumentEventItem_MarketDocumentEventItemLevel_LevelId",
                table: "MarketDocumentEventItem",
                column: "LevelId",
                principalTable: "MarketDocumentEventItemLevel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessRunLogItem_ProcessRunLogLevel_LevelId",
                table: "ProcessRunLogItem",
                column: "LevelId",
                principalTable: "ProcessRunLogLevel",
                principalColumn: "Id");
        }
    }
}

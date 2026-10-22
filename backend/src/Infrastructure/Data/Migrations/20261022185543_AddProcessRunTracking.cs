using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessRunTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "ProcessRunStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessRunStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessRun",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    StartedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CompletedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessRun", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessRun_ProcessRunStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ProcessRunStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProcessRun_ProcessType_ProcessTypeId",
                        column: x => x.ProcessTypeId,
                        principalTable: "ProcessType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProcessRunLogItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessRunId = table.Column<long>(type: "bigint", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Field = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessRunLogItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessRunLogItem_ProcessRunLogLevel_LevelId",
                        column: x => x.LevelId,
                        principalTable: "ProcessRunLogLevel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProcessRunLogItem_ProcessRun_ProcessRunId",
                        column: x => x.ProcessRunId,
                        principalTable: "ProcessRun",
                        principalColumn: "Id");
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

            migrationBuilder.InsertData(
                table: "ProcessRunStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Running" },
                    { 2, "Succeeded" },
                    { 3, "Failed" }
                });

            migrationBuilder.InsertData(
                table: "ProcessType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "MarketDocumentImport" },
                    { 2, "MarketDocumentExport" },
                    { 3, "MarketDocumentItemExport" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessRun_ProcessTypeId",
                table: "ProcessRun",
                column: "ProcessTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessRun_StatusId",
                table: "ProcessRun",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessRunLogItem_EntityType_EntityId",
                table: "ProcessRunLogItem",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessRunLogItem_LevelId",
                table: "ProcessRunLogItem",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessRunLogItem_ProcessRunId",
                table: "ProcessRunLogItem",
                column: "ProcessRunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessRunLogItem");

            migrationBuilder.DropTable(
                name: "ProcessRunLogLevel");

            migrationBuilder.DropTable(
                name: "ProcessRun");

            migrationBuilder.DropTable(
                name: "ProcessRunStatus");

            migrationBuilder.DropTable(
                name: "ProcessType");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceErrorLoggingWithCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketDocumentEventItem");

            migrationBuilder.DropTable(
                name: "ProcessRunLogItem");

            migrationBuilder.CreateTable(
                name: "CaseEntityType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseEntityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Case",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityTypeId = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    AssignedToUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Case", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Case_CaseEntityType_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "CaseEntityType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Case_CaseStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "CaseStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Case_User_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CaseItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseId = table.Column<long>(type: "bigint", nullable: false),
                    SeverityId = table.Column<int>(type: "int", nullable: false),
                    Field = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseItem_Case_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Case",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseItem_ErrorSeverity_SeverityId",
                        column: x => x.SeverityId,
                        principalTable: "ErrorSeverity",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "CaseEntityType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "ProcessRun" },
                    { 2, "MarketDocumentItem" },
                    { 3, "MarketDocument" }
                });

            migrationBuilder.InsertData(
                table: "CaseStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Open" },
                    { 2, "InProgress" },
                    { 3, "Resolved" },
                    { 4, "Cancelled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Case_AssignedToUserId",
                table: "Case",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Case_EntityTypeId",
                table: "Case",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Case_StatusId",
                table: "Case",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseItem_CaseId",
                table: "CaseItem",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseItem_SeverityId",
                table: "CaseItem",
                column: "SeverityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseItem");

            migrationBuilder.DropTable(
                name: "Case");

            migrationBuilder.DropTable(
                name: "CaseEntityType");

            migrationBuilder.DropTable(
                name: "CaseStatus");

            migrationBuilder.CreateTable(
                name: "MarketDocumentEventItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketDocumentEventId = table.Column<long>(type: "bigint", nullable: false),
                    SeverityId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Field = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentEventItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketDocumentEventItem_ErrorSeverity_SeverityId",
                        column: x => x.SeverityId,
                        principalTable: "ErrorSeverity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarketDocumentEventItem_MarketDocumentEvent_MarketDocumentEventId",
                        column: x => x.MarketDocumentEventId,
                        principalTable: "MarketDocumentEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessRunLogItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessRunId = table.Column<long>(type: "bigint", nullable: false),
                    SeverityId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Field = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessRunLogItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessRunLogItem_ErrorSeverity_SeverityId",
                        column: x => x.SeverityId,
                        principalTable: "ErrorSeverity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProcessRunLogItem_ProcessRun_ProcessRunId",
                        column: x => x.ProcessRunId,
                        principalTable: "ProcessRun",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentEventItem_MarketDocumentEventId",
                table: "MarketDocumentEventItem",
                column: "MarketDocumentEventId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentEventItem_SeverityId",
                table: "MarketDocumentEventItem",
                column: "SeverityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessRunLogItem_ProcessRunId",
                table: "ProcessRunLogItem",
                column: "ProcessRunId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessRunLogItem_SeverityId",
                table: "ProcessRunLogItem",
                column: "SeverityId");
        }
    }
}

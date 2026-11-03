using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceMarketDocumentReferenceWithMarketDocumentEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketDocumentReference");

            migrationBuilder.DropTable(
                name: "MarketDocumentReferenceType");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "MarketDocumentItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "MarketDocument",
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
                name: "MarketDocumentEventStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentEventStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketDocumentEventType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentEventType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketDocumentEvent",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketDocumentItemId = table.Column<long>(type: "bigint", nullable: false),
                    EventTypeId = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketDocumentEvent_MarketDocumentEventStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "MarketDocumentEventStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarketDocumentEvent_MarketDocumentEventType_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "MarketDocumentEventType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarketDocumentEvent_MarketDocumentItem_MarketDocumentItemId",
                        column: x => x.MarketDocumentItemId,
                        principalTable: "MarketDocumentItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketDocumentEventItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketDocumentEventId = table.Column<long>(type: "bigint", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    Field = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentEventItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketDocumentEventItem_MarketDocumentEventItemLevel_LevelId",
                        column: x => x.LevelId,
                        principalTable: "MarketDocumentEventItemLevel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarketDocumentEventItem_MarketDocumentEvent_MarketDocumentEventId",
                        column: x => x.MarketDocumentEventId,
                        principalTable: "MarketDocumentEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                table: "MarketDocumentEventStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Done" },
                    { 2, "Error" }
                });

            migrationBuilder.InsertData(
                table: "MarketDocumentEventType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "HistoricalUsage" },
                    { 2, "Usage" },
                    { 3, "Invoice" },
                    { 4, "ServiceStart" },
                    { 5, "ServiceEnd" },
                    { 6, "ServiceRequest" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentEvent_EventTypeId",
                table: "MarketDocumentEvent",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentEvent_MarketDocumentItemId",
                table: "MarketDocumentEvent",
                column: "MarketDocumentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentEvent_StatusId",
                table: "MarketDocumentEvent",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentEventItem_LevelId",
                table: "MarketDocumentEventItem",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentEventItem_MarketDocumentEventId",
                table: "MarketDocumentEventItem",
                column: "MarketDocumentEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketDocumentEventItem");

            migrationBuilder.DropTable(
                name: "MarketDocumentEventItemLevel");

            migrationBuilder.DropTable(
                name: "MarketDocumentEvent");

            migrationBuilder.DropTable(
                name: "MarketDocumentEventStatus");

            migrationBuilder.DropTable(
                name: "MarketDocumentEventType");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "MarketDocumentItem");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "MarketDocument");

            migrationBuilder.CreateTable(
                name: "MarketDocumentReferenceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentReferenceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketDocumentReference",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketDocumentItemId = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketDocumentReference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketDocumentReference_MarketDocumentItem_MarketDocumentItemId",
                        column: x => x.MarketDocumentItemId,
                        principalTable: "MarketDocumentItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketDocumentReference_MarketDocumentReferenceType_ReferenceTypeId",
                        column: x => x.ReferenceTypeId,
                        principalTable: "MarketDocumentReferenceType",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "MarketDocumentReferenceType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "HistoricalUsage" },
                    { 2, "Usage" },
                    { 3, "Invoice" },
                    { 4, "ServiceStart" },
                    { 5, "ServiceEnd" },
                    { 6, "ServiceRequest" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentReference_MarketDocumentItemId",
                table: "MarketDocumentReference",
                column: "MarketDocumentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketDocumentReference_ReferenceTypeId",
                table: "MarketDocumentReference",
                column: "ReferenceTypeId");
        }
    }
}

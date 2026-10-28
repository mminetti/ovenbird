using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceEventEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceEventActionType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEventActionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEventReferenceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEventReferenceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEventType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEventType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEvent",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventTypeId = table.Column<int>(type: "int", nullable: false),
                    ReferenceTypeId = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceEvent_ServiceEventReferenceType_ReferenceTypeId",
                        column: x => x.ReferenceTypeId,
                        principalTable: "ServiceEventReferenceType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceEvent_ServiceEventType_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "ServiceEventType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceEventAction",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceEventId = table.Column<long>(type: "bigint", nullable: false),
                    ActionTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEventAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceEventAction_ServiceEventActionType_ActionTypeId",
                        column: x => x.ActionTypeId,
                        principalTable: "ServiceEventActionType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceEventAction_ServiceEvent_ServiceEventId",
                        column: x => x.ServiceEventId,
                        principalTable: "ServiceEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceEventAction_ServiceOrderStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ServiceOrderStatus",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ServiceEventActionType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "UpdateServicePoint" },
                    { 2, "SendNotification" }
                });

            migrationBuilder.InsertData(
                table: "ServiceEventReferenceType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "ServiceStartItem" });

            migrationBuilder.InsertData(
                table: "ServiceEventType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "ServiceStartConfirmed" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEvent_EventTypeId",
                table: "ServiceEvent",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEvent_ReferenceTypeId",
                table: "ServiceEvent",
                column: "ReferenceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEventAction_ActionTypeId",
                table: "ServiceEventAction",
                column: "ActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEventAction_ServiceEventId",
                table: "ServiceEventAction",
                column: "ServiceEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEventAction_StatusId",
                table: "ServiceEventAction",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceEventAction");

            migrationBuilder.DropTable(
                name: "ServiceEventActionType");

            migrationBuilder.DropTable(
                name: "ServiceEvent");

            migrationBuilder.DropTable(
                name: "ServiceEventReferenceType");

            migrationBuilder.DropTable(
                name: "ServiceEventType");
        }
    }
}

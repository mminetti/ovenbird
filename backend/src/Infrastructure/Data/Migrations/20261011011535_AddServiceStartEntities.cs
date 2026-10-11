using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceStartEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceAction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceStartType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceStartType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceStart",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceStartTypeId = table.Column<int>(type: "int", nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ServiceStatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceStart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceStart_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceStart_ServiceStartType_ServiceStartTypeId",
                        column: x => x.ServiceStartTypeId,
                        principalTable: "ServiceStartType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceStart_ServiceStatus_ServiceStatusId",
                        column: x => x.ServiceStatusId,
                        principalTable: "ServiceStatus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceStartItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceStartId = table.Column<long>(type: "bigint", nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ServiceActionId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceStartItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceStartItem_ServiceAction_ServiceActionId",
                        column: x => x.ServiceActionId,
                        principalTable: "ServiceAction",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceStartItem_ServiceStart_ServiceStartId",
                        column: x => x.ServiceStartId,
                        principalTable: "ServiceStart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ServiceAction",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Initiate" },
                    { 2, "Confirm" },
                    { 3, "Reject" },
                    { 4, "Complete" },
                    { 5, "Cancel" },
                    { 6, "Unexecute" },
                    { 7, "Error" },
                    { 8, "Change" }
                });

            migrationBuilder.InsertData(
                table: "ServiceStartType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "MoveIn" },
                    { 2, "Switch" }
                });

            migrationBuilder.InsertData(
                table: "ServiceStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "New" },
                    { 2, "InProgress" },
                    { 3, "Scheduled" },
                    { 4, "Cancelled" },
                    { 5, "Rejected" },
                    { 6, "Completed" },
                    { 7, "Unexecuted" },
                    { 8, "Error" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceStart_AccountId",
                table: "ServiceStart",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceStart_ServiceStartTypeId",
                table: "ServiceStart",
                column: "ServiceStartTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceStart_ServiceStatusId",
                table: "ServiceStart",
                column: "ServiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceStartItem_ServiceActionId",
                table: "ServiceStartItem",
                column: "ServiceActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceStartItem_ServiceStartId",
                table: "ServiceStartItem",
                column: "ServiceStartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceStartItem");

            migrationBuilder.DropTable(
                name: "ServiceAction");

            migrationBuilder.DropTable(
                name: "ServiceStart");

            migrationBuilder.DropTable(
                name: "ServiceStartType");

            migrationBuilder.DropTable(
                name: "ServiceStatus");
        }
    }
}

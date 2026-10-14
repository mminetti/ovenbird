using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveServiceFieldsFromAccountToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_ServiceConnectionStatus_ServiceConnectionStatusId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_ServiceStatus_ServiceStatusId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_ServiceSubStatus_ServiceSubStatusId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ServiceConnectionStatusId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ServiceStatusId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ServiceSubStatusId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "IsServiceLocked",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "IsServiceProtected",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ServiceConnectionStatusId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ServiceEndDate",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ServiceStartDate",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ServiceStatusId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ServiceSubStatusId",
                table: "Account");

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    SubStatusId = table.Column<int>(type: "int", nullable: false),
                    ConnectionStatusId = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    IsProtected = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Service_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Service_ServiceConnectionStatus_ConnectionStatusId",
                        column: x => x.ConnectionStatusId,
                        principalTable: "ServiceConnectionStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Service_ServiceStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ServiceStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Service_ServiceSubStatus_SubStatusId",
                        column: x => x.SubStatusId,
                        principalTable: "ServiceSubStatus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Service_AccountId",
                table: "Service",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Service_ConnectionStatusId",
                table: "Service",
                column: "ConnectionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_StatusId",
                table: "Service",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_SubStatusId",
                table: "Service",
                column: "SubStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.AddColumn<bool>(
                name: "IsServiceLocked",
                table: "Account",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsServiceProtected",
                table: "Account",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ServiceConnectionStatusId",
                table: "Account",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ServiceEndDate",
                table: "Account",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ServiceStartDate",
                table: "Account",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "ServiceStatusId",
                table: "Account",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceSubStatusId",
                table: "Account",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Account_ServiceConnectionStatusId",
                table: "Account",
                column: "ServiceConnectionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_ServiceStatusId",
                table: "Account",
                column: "ServiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_ServiceSubStatusId",
                table: "Account",
                column: "ServiceSubStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ServiceConnectionStatus_ServiceConnectionStatusId",
                table: "Account",
                column: "ServiceConnectionStatusId",
                principalTable: "ServiceConnectionStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ServiceStatus_ServiceStatusId",
                table: "Account",
                column: "ServiceStatusId",
                principalTable: "ServiceStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ServiceSubStatus_ServiceSubStatusId",
                table: "Account",
                column: "ServiceSubStatusId",
                principalTable: "ServiceSubStatus",
                principalColumn: "Id");
        }
    }
}

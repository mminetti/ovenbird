using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceSubscriptionEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceStart_ServiceStatus_ServiceStatusId",
                table: "ServiceStart");

            migrationBuilder.DeleteData(
                table: "ServiceStatus",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ServiceStatus",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ServiceStatus",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ServiceStatus",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ServiceStatus",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.RenameColumn(
                name: "ServiceStatusId",
                table: "ServiceStart",
                newName: "ServiceOrderStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceStart_ServiceStatusId",
                table: "ServiceStart",
                newName: "IX_ServiceStart_ServiceOrderStatusId");

            migrationBuilder.AddColumn<int>(
                name: "CommodityId",
                table: "ServicePoint",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Cycle",
                table: "ServicePoint",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCriticalCare",
                table: "ServicePoint",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MarketId",
                table: "ServicePoint",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "PremiseId",
                table: "ServicePoint",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "PremiseTypeId",
                table: "ServicePoint",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UtilityId",
                table: "ServicePoint",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Account",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "CustomerId",
                table: "Account",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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

            migrationBuilder.AddColumn<long>(
                name: "ServicePointId",
                table: "Account",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ServiceStartDate",
                table: "Account",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<long>(
                name: "ServiceStartId",
                table: "Account",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identifier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Premise",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Address2 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    City = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    State = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Zip = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    County = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Premise", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PremiseType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiseType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceConnectionStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceConnectionStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOrderStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrderStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSubStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSubStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utility",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    MarketId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Utility_Market_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Market",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ServiceConnectionStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "PendingDisconnection" },
                    { 2, "Disconnected" },
                    { 3, "PendingConnection" },
                    { 4, "Connected" }
                });

            migrationBuilder.InsertData(
                table: "ServiceOrderStatus",
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

            migrationBuilder.UpdateData(
                table: "ServiceStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Pending");

            migrationBuilder.UpdateData(
                table: "ServiceStatus",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "On");

            migrationBuilder.UpdateData(
                table: "ServiceStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Off");

            migrationBuilder.InsertData(
                table: "ServiceSubStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "PendingStart" },
                    { 2, "Active" },
                    { 3, "PendingEnd" },
                    { 4, "Ended" },
                    { 5, "Closed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicePoint_CommodityId",
                table: "ServicePoint",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePoint_MarketId",
                table: "ServicePoint",
                column: "MarketId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePoint_PremiseId",
                table: "ServicePoint",
                column: "PremiseId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePoint_PremiseTypeId",
                table: "ServicePoint",
                column: "PremiseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePoint_UtilityId",
                table: "ServicePoint",
                column: "UtilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_CustomerId",
                table: "Account",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_ServiceConnectionStatusId",
                table: "Account",
                column: "ServiceConnectionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_ServicePointId",
                table: "Account",
                column: "ServicePointId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_ServiceStartId",
                table: "Account",
                column: "ServiceStartId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_ServiceStatusId",
                table: "Account",
                column: "ServiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_ServiceSubStatusId",
                table: "Account",
                column: "ServiceSubStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Utility_MarketId",
                table: "Utility",
                column: "MarketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Customer_CustomerId",
                table: "Account",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ServiceConnectionStatus_ServiceConnectionStatusId",
                table: "Account",
                column: "ServiceConnectionStatusId",
                principalTable: "ServiceConnectionStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ServicePoint_ServicePointId",
                table: "Account",
                column: "ServicePointId",
                principalTable: "ServicePoint",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_ServiceStart_ServiceStartId",
                table: "Account",
                column: "ServiceStartId",
                principalTable: "ServiceStart",
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

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePoint_Commodity_CommodityId",
                table: "ServicePoint",
                column: "CommodityId",
                principalTable: "Commodity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePoint_Market_MarketId",
                table: "ServicePoint",
                column: "MarketId",
                principalTable: "Market",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePoint_PremiseType_PremiseTypeId",
                table: "ServicePoint",
                column: "PremiseTypeId",
                principalTable: "PremiseType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePoint_Premise_PremiseId",
                table: "ServicePoint",
                column: "PremiseId",
                principalTable: "Premise",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePoint_Utility_UtilityId",
                table: "ServicePoint",
                column: "UtilityId",
                principalTable: "Utility",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceStart_ServiceOrderStatus_ServiceOrderStatusId",
                table: "ServiceStart",
                column: "ServiceOrderStatusId",
                principalTable: "ServiceOrderStatus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Customer_CustomerId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_ServiceConnectionStatus_ServiceConnectionStatusId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_ServicePoint_ServicePointId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_ServiceStart_ServiceStartId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_ServiceStatus_ServiceStatusId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_ServiceSubStatus_ServiceSubStatusId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicePoint_Commodity_CommodityId",
                table: "ServicePoint");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicePoint_Market_MarketId",
                table: "ServicePoint");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicePoint_PremiseType_PremiseTypeId",
                table: "ServicePoint");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicePoint_Premise_PremiseId",
                table: "ServicePoint");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicePoint_Utility_UtilityId",
                table: "ServicePoint");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceStart_ServiceOrderStatus_ServiceOrderStatusId",
                table: "ServiceStart");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Premise");

            migrationBuilder.DropTable(
                name: "PremiseType");

            migrationBuilder.DropTable(
                name: "ServiceConnectionStatus");

            migrationBuilder.DropTable(
                name: "ServiceOrderStatus");

            migrationBuilder.DropTable(
                name: "ServiceSubStatus");

            migrationBuilder.DropTable(
                name: "Utility");

            migrationBuilder.DropIndex(
                name: "IX_ServicePoint_CommodityId",
                table: "ServicePoint");

            migrationBuilder.DropIndex(
                name: "IX_ServicePoint_MarketId",
                table: "ServicePoint");

            migrationBuilder.DropIndex(
                name: "IX_ServicePoint_PremiseId",
                table: "ServicePoint");

            migrationBuilder.DropIndex(
                name: "IX_ServicePoint_PremiseTypeId",
                table: "ServicePoint");

            migrationBuilder.DropIndex(
                name: "IX_ServicePoint_UtilityId",
                table: "ServicePoint");

            migrationBuilder.DropIndex(
                name: "IX_Account_CustomerId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ServiceConnectionStatusId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ServicePointId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ServiceStartId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ServiceStatusId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ServiceSubStatusId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "CommodityId",
                table: "ServicePoint");

            migrationBuilder.DropColumn(
                name: "Cycle",
                table: "ServicePoint");

            migrationBuilder.DropColumn(
                name: "IsCriticalCare",
                table: "ServicePoint");

            migrationBuilder.DropColumn(
                name: "MarketId",
                table: "ServicePoint");

            migrationBuilder.DropColumn(
                name: "PremiseId",
                table: "ServicePoint");

            migrationBuilder.DropColumn(
                name: "PremiseTypeId",
                table: "ServicePoint");

            migrationBuilder.DropColumn(
                name: "UtilityId",
                table: "ServicePoint");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "CustomerId",
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
                name: "ServicePointId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ServiceStartDate",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ServiceStartId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ServiceStatusId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ServiceSubStatusId",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "ServiceOrderStatusId",
                table: "ServiceStart",
                newName: "ServiceStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceStart_ServiceOrderStatusId",
                table: "ServiceStart",
                newName: "IX_ServiceStart_ServiceStatusId");

            migrationBuilder.UpdateData(
                table: "ServiceStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "New");

            migrationBuilder.UpdateData(
                table: "ServiceStatus",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "InProgress");

            migrationBuilder.UpdateData(
                table: "ServiceStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Scheduled");

            migrationBuilder.InsertData(
                table: "ServiceStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 4, "Cancelled" },
                    { 5, "Rejected" },
                    { 6, "Completed" },
                    { 7, "Unexecuted" },
                    { 8, "Error" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceStart_ServiceStatus_ServiceStatusId",
                table: "ServiceStart",
                column: "ServiceStatusId",
                principalTable: "ServiceStatus",
                principalColumn: "Id");
        }
    }
}

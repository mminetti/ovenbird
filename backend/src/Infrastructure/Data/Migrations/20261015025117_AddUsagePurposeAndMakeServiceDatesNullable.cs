using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsagePurposeAndMakeServiceDatesNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UsageType",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "UsagePurposeId",
                table: "Usage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "Service",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "Service",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateTable(
                name: "UsagePurpose",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsagePurpose", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "UsagePurpose",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Original" },
                    { 2, "Cancel" },
                    { 3, "Replace" }
                });

            migrationBuilder.UpdateData(
                table: "UsageType",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "NonInterval");

            migrationBuilder.UpdateData(
                table: "UsageType",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Interval");

            migrationBuilder.CreateIndex(
                name: "IX_Usage_UsagePurposeId",
                table: "Usage",
                column: "UsagePurposeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usage_UsagePurpose_UsagePurposeId",
                table: "Usage",
                column: "UsagePurposeId",
                principalTable: "UsagePurpose",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usage_UsagePurpose_UsagePurposeId",
                table: "Usage");

            migrationBuilder.DropTable(
                name: "UsagePurpose");

            migrationBuilder.DropIndex(
                name: "IX_Usage_UsagePurposeId",
                table: "Usage");

            migrationBuilder.DropColumn(
                name: "UsagePurposeId",
                table: "Usage");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StartDate",
                table: "Service",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndDate",
                table: "Service",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "UsageType",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Original");

            migrationBuilder.UpdateData(
                table: "UsageType",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Cancel");

            migrationBuilder.InsertData(
                table: "UsageType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Replace" });
        }
    }
}

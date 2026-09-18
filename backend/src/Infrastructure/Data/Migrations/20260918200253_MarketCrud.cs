using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MarketCrud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_Market_MarketId",
                table: "Company");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Market",
                table: "Market");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Market");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Market",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Market",
                table: "Market",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Market_MarketId",
                table: "Company",
                column: "MarketId",
                principalTable: "Market",
                principalColumn: "Id");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                table: "Market",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Market",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedAtUtc",
                table: "Market",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Market",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 11, "Allows viewing markets.", "markets.read" },
                    { 12, "Allows creating, updating, and deleting markets.", "markets.write" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Market");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Market");

            migrationBuilder.DropColumn(
                name: "LastModifiedAtUtc",
                table: "Market");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Market");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_Market_MarketId",
                table: "Company");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Market",
                table: "Market");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Market");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Market",
                type: "int",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Market",
                table: "Market",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Market_MarketId",
                table: "Company",
                column: "MarketId",
                principalTable: "Market",
                principalColumn: "Id");
        }
    }
}

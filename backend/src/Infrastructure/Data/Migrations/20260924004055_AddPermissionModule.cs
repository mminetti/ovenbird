using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "Permission",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PermissionModule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionModule", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1,
                column: "ModuleId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 2,
                column: "ModuleId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 3,
                column: "ModuleId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 4,
                column: "ModuleId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5,
                column: "ModuleId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 6,
                column: "ModuleId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 7,
                column: "ModuleId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 8,
                column: "ModuleId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 9,
                column: "ModuleId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 10,
                column: "ModuleId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 11,
                column: "ModuleId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 12,
                column: "ModuleId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 13,
                column: "ModuleId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 14,
                column: "ModuleId",
                value: 2);

            migrationBuilder.InsertData(
                table: "PermissionModule",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Security" },
                    { 2, "Settings" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permission_ModuleId",
                table: "Permission",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permission_PermissionModule_ModuleId",
                table: "Permission",
                column: "ModuleId",
                principalTable: "PermissionModule",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permission_PermissionModule_ModuleId",
                table: "Permission");

            migrationBuilder.DropTable(
                name: "PermissionModule");

            migrationBuilder.DropIndex(
                name: "IX_Permission_ModuleId",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Permission");
        }
    }
}

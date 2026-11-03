using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProcessRunLogItemEntityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProcessRunLogItem_EntityType_EntityId",
                table: "ProcessRunLogItem");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "ProcessRunLogItem");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "ProcessRunLogItem");

            migrationBuilder.AlterColumn<string>(
                name: "Field",
                table: "ProcessRunLogItem",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Field",
                table: "ProcessRunLogItem",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                table: "ProcessRunLogItem",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "ProcessRunLogItem",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessRunLogItem_EntityType_EntityId",
                table: "ProcessRunLogItem",
                columns: new[] { "EntityType", "EntityId" });
        }
    }
}

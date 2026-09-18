using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurationTypeDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Configuration_ConfigurationType_ConfigurationTypeId",
                table: "Configuration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConfigurationType",
                table: "ConfigurationType");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ConfigurationType");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ConfigurationType",
                type: "int",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ConfigurationType",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConfigurationType",
                table: "ConfigurationType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Configuration_ConfigurationType_ConfigurationTypeId",
                table: "Configuration",
                column: "ConfigurationTypeId",
                principalTable: "ConfigurationType",
                principalColumn: "Id");

            migrationBuilder.InsertData(
                table: "ConfigurationType",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, null, "EDI Import" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConfigurationType",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropForeignKey(
                name: "FK_Configuration_ConfigurationType_ConfigurationTypeId",
                table: "Configuration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConfigurationType",
                table: "ConfigurationType");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ConfigurationType");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ConfigurationType");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ConfigurationType",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConfigurationType",
                table: "ConfigurationType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Configuration_ConfigurationType_ConfigurationTypeId",
                table: "Configuration",
                column: "ConfigurationTypeId",
                principalTable: "ConfigurationType",
                principalColumn: "Id");
        }
    }
}

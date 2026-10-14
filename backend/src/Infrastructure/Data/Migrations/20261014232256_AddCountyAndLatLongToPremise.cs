using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCountyAndLatLongToPremise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "County",
                table: "Premise");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Premise",
                newName: "AddressLine1");

            migrationBuilder.RenameColumn(
                name: "Address2",
                table: "Premise",
                newName: "AddressLine2");

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine2",
                table: "Premise",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AddColumn<string>(
                name: "Zip4",
                table: "Premise",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountyId",
                table: "Premise",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Premise",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Premise",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "County",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_County", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Premise_CountyId",
                table: "Premise",
                column: "CountyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Premise_County_CountyId",
                table: "Premise",
                column: "CountyId",
                principalTable: "County",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Premise_County_CountyId",
                table: "Premise");

            migrationBuilder.DropTable(
                name: "County");

            migrationBuilder.DropIndex(
                name: "IX_Premise_CountyId",
                table: "Premise");

            migrationBuilder.DropColumn(
                name: "Zip4",
                table: "Premise");

            migrationBuilder.DropColumn(
                name: "CountyId",
                table: "Premise");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Premise");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Premise");

            migrationBuilder.AlterColumn<string>(
                name: "AddressLine2",
                table: "Premise",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.RenameColumn(
                name: "AddressLine2",
                table: "Premise",
                newName: "Address2");

            migrationBuilder.RenameColumn(
                name: "AddressLine1",
                table: "Premise",
                newName: "Address");

            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "Premise",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}

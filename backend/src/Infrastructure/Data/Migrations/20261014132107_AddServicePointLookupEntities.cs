using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServicePointLookupEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicePoint_PremiseType_PremiseTypeId",
                table: "ServicePoint");

            migrationBuilder.DropTable(
                name: "PremiseType");

            migrationBuilder.DropColumn(
                name: "Cycle",
                table: "ServicePoint");

            migrationBuilder.RenameColumn(
                name: "PremiseTypeId",
                table: "ServicePoint",
                newName: "ReadCycleId");

            migrationBuilder.RenameIndex(
                name: "IX_ServicePoint_PremiseTypeId",
                table: "ServicePoint",
                newName: "IX_ServicePoint_ReadCycleId");

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "Usage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "ServiceStart",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerClassId",
                table: "ServicePoint",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAmsMeter",
                table: "ServicePoint",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LoadProfileId",
                table: "ServicePoint",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "HistoricalUsage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerClass", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoadProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReadCycle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadCycle", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicePoint_CustomerClassId",
                table: "ServicePoint",
                column: "CustomerClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePoint_LoadProfileId",
                table: "ServicePoint",
                column: "LoadProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePoint_CustomerClass_CustomerClassId",
                table: "ServicePoint",
                column: "CustomerClassId",
                principalTable: "CustomerClass",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePoint_LoadProfile_LoadProfileId",
                table: "ServicePoint",
                column: "LoadProfileId",
                principalTable: "LoadProfile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePoint_ReadCycle_ReadCycleId",
                table: "ServicePoint",
                column: "ReadCycleId",
                principalTable: "ReadCycle",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicePoint_CustomerClass_CustomerClassId",
                table: "ServicePoint");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicePoint_LoadProfile_LoadProfileId",
                table: "ServicePoint");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicePoint_ReadCycle_ReadCycleId",
                table: "ServicePoint");

            migrationBuilder.DropTable(
                name: "CustomerClass");

            migrationBuilder.DropTable(
                name: "LoadProfile");

            migrationBuilder.DropTable(
                name: "ReadCycle");

            migrationBuilder.DropIndex(
                name: "IX_ServicePoint_CustomerClassId",
                table: "ServicePoint");

            migrationBuilder.DropIndex(
                name: "IX_ServicePoint_LoadProfileId",
                table: "ServicePoint");

            migrationBuilder.DropColumn(
                name: "CustomerClassId",
                table: "ServicePoint");

            migrationBuilder.DropColumn(
                name: "IsAmsMeter",
                table: "ServicePoint");

            migrationBuilder.DropColumn(
                name: "LoadProfileId",
                table: "ServicePoint");

            migrationBuilder.RenameColumn(
                name: "ReadCycleId",
                table: "ServicePoint",
                newName: "PremiseTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServicePoint_ReadCycleId",
                table: "ServicePoint",
                newName: "IX_ServicePoint_PremiseTypeId");

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "Usage",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "ServiceStart",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Cycle",
                table: "ServicePoint",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "HistoricalUsage",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePoint_PremiseType_PremiseTypeId",
                table: "ServicePoint",
                column: "PremiseTypeId",
                principalTable: "PremiseType",
                principalColumn: "Id");
        }
    }
}

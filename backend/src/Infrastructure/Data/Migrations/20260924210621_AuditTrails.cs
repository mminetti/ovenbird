using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AuditTrails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditTrail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TimestampUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AffectedColumns = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditTrailReference",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferencedEntityType = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ReferencedEntityId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AuditTrailId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrailReference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditTrailReference_AuditTrail_AuditTrailId",
                        column: x => x.AuditTrailId,
                        principalTable: "AuditTrail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Description", "ModuleId", "Name" },
                values: new object[] { 15, "Allows viewing the audit trail.", 1, "audit.trails.read" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrail_EntityType_EntityId",
                table: "AuditTrail",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailReference_AuditTrailId",
                table: "AuditTrailReference",
                column: "AuditTrailId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrailReference_ReferencedEntityType_ReferencedEntityId",
                table: "AuditTrailReference",
                columns: new[] { "ReferencedEntityType", "ReferencedEntityId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrailReference");

            migrationBuilder.DropTable(
                name: "AuditTrail");

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}

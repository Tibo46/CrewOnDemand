using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CrewOnDemand.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bases",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pilots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    BaseId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pilots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pilots_Bases_BaseId",
                        column: x => x.BaseId,
                        principalTable: "Bases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PilotId = table.Column<int>(nullable: false),
                    DepartureDateTime = table.Column<DateTime>(nullable: false),
                    ReturnDateTime = table.Column<DateTime>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Pilots_PilotId",
                        column: x => x.PilotId,
                        principalTable: "Pilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkDays",
                columns: table => new
                {
                    PilotId = table.Column<int>(nullable: false),
                    DayOfTheWeek = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkDays", x => new { x.DayOfTheWeek, x.PilotId });
                    table.ForeignKey(
                        name: "FK_WorkDays_Pilots_PilotId",
                        column: x => x.PilotId,
                        principalTable: "Pilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Bases",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "munich" });

            migrationBuilder.InsertData(
                table: "Bases",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "berlin" });

            migrationBuilder.InsertData(
                table: "Pilots",
                columns: new[] { "Id", "BaseId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Andy" },
                    { 2, 1, "Betty" },
                    { 3, 1, "Callum" },
                    { 4, 1, "Daphne" },
                    { 5, 2, "Elvis" },
                    { 6, 2, "Freida" },
                    { 7, 2, "Greg" },
                    { 8, 2, "Hermione" }
                });

            migrationBuilder.InsertData(
                table: "WorkDays",
                columns: new[] { "DayOfTheWeek", "PilotId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 5, 8 },
                    { 0, 7 },
                    { 6, 7 },
                    { 4, 7 },
                    { 3, 7 },
                    { 5, 6 },
                    { 3, 6 },
                    { 2, 6 },
                    { 1, 6 },
                    { 6, 5 },
                    { 4, 5 },
                    { 2, 5 },
                    { 1, 5 },
                    { 0, 4 },
                    { 6, 4 },
                    { 5, 4 },
                    { 0, 3 },
                    { 6, 3 },
                    { 4, 3 },
                    { 3, 3 },
                    { 5, 2 },
                    { 3, 2 },
                    { 2, 2 },
                    { 1, 2 },
                    { 6, 1 },
                    { 4, 1 },
                    { 2, 1 },
                    { 6, 8 },
                    { 0, 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PilotId",
                table: "Bookings",
                column: "PilotId");

            migrationBuilder.CreateIndex(
                name: "IX_Pilots_BaseId",
                table: "Pilots",
                column: "BaseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkDays_PilotId",
                table: "WorkDays",
                column: "PilotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "WorkDays");

            migrationBuilder.DropTable(
                name: "Pilots");

            migrationBuilder.DropTable(
                name: "Bases");
        }
    }
}

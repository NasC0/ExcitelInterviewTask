using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Leads.Database.Ef.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PinCode = table.Column<string>(nullable: false),
                    SubAreaId = table.Column<int>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    MobileNumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    PinCode = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubAreas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Id",
                table: "Leads",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubAreas_Id",
                table: "SubAreas",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubAreas_PinCode",
                table: "SubAreas",
                column: "PinCode");


            migrationBuilder.InsertData(
                table: "SubAreas",
                columns: new[] { "Id", "PinCode", "Name" },
                values: new object[] {1, "123", "Name1"});
            migrationBuilder.InsertData(
                table: "SubAreas",
                columns: new[] { "Id", "PinCode", "Name" },
                values: new object[] {2, "123", "Name2"});
            migrationBuilder.InsertData(
                table: "SubAreas",
                columns: new[] { "Id", "PinCode", "Name" },
                values: new object[] {3, "123", "Name3"});

            migrationBuilder.InsertData(
                table: "SubAreas",
                columns: new[] { "Id", "PinCode", "Name" },
                values: new object[] {4, "567", "Name4"});
            migrationBuilder.InsertData(
                table: "SubAreas",
                columns: new[] { "Id", "PinCode", "Name" },
                values: new object[] {5, "567", "Name5"});
            migrationBuilder.InsertData(
                table: "SubAreas",
                columns: new[] { "Id", "PinCode", "Name" },
                values: new object[] {6, "567", "Name6"});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "SubAreas");
        }
    }
}

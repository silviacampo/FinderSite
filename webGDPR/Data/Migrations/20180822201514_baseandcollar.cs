using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class baseandcollar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Base",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsConnected = table.Column<bool>(nullable: false),
                    IsPlugged = table.Column<bool>(nullable: false),
                    IsCharging = table.Column<bool>(nullable: false),
                    Battery = table.Column<int>(nullable: false),
                    HasBattery = table.Column<bool>(nullable: false),
                    Radio = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Base", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collar",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsConnected = table.Column<bool>(nullable: false),
                    IsGPSConnected = table.Column<bool>(nullable: false),
                    Battery = table.Column<int>(nullable: false),
                    Radio = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collar", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Base");

            migrationBuilder.DropTable(
                name: "Collar");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class basestatus3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaseStatus",
                columns: table => new
                {
                    BaseStatusId = table.Column<string>(nullable: false),
                    BaseId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsConnected = table.Column<bool>(nullable: false),
                    IsPlugged = table.Column<bool>(nullable: false),
                    IsCharging = table.Column<bool>(nullable: false),
                    Battery = table.Column<int>(nullable: false),
                    HasBattery = table.Column<bool>(nullable: false),
                    Radio = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseStatus", x => x.BaseStatusId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseStatus");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class basestatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Battery",
                table: "Base");

            migrationBuilder.DropColumn(
                name: "HasBattery",
                table: "Base");

            migrationBuilder.DropColumn(
                name: "IsCharging",
                table: "Base");

            migrationBuilder.DropColumn(
                name: "IsConnected",
                table: "Base");

            migrationBuilder.DropColumn(
                name: "IsPlugged",
                table: "Base");

            migrationBuilder.DropColumn(
                name: "Radio",
                table: "Base");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Battery",
                table: "Base",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasBattery",
                table: "Base",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCharging",
                table: "Base",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsConnected",
                table: "Base",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlugged",
                table: "Base",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Radio",
                table: "Base",
                nullable: false,
                defaultValue: 0);
        }
    }
}

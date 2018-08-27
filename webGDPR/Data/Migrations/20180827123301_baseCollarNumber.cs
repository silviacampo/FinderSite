using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class baseCollarNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "BaseNumber",
                table: "Collar",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "CollarNumber",
                table: "Collar",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "BaseNumber",
                table: "Base",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseNumber",
                table: "Collar");

            migrationBuilder.DropColumn(
                name: "CollarNumber",
                table: "Collar");

            migrationBuilder.DropColumn(
                name: "BaseNumber",
                table: "Base");
        }
    }
}

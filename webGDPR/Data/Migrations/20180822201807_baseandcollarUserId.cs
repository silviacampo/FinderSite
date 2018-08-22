using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class baseandcollarUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Collar",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Base",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Collar");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Base");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class petmode2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseId",
                table: "PetMode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaseId",
                table: "PetMode",
                nullable: true);
        }
    }
}

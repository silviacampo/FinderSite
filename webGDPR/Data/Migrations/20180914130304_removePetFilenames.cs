using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class removePetFilenames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Pet");

            migrationBuilder.DropColumn(
                name: "PageFileName",
                table: "Pet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Pet",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageFileName",
                table: "Pet",
                nullable: true);
        }
    }
}

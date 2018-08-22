using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class baseandcollarHWId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Collar",
                newName: "CollarId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Base",
                newName: "BaseId");

            migrationBuilder.AddColumn<string>(
                name: "HWId",
                table: "Collar",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HWId",
                table: "Base",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HWId",
                table: "Collar");

            migrationBuilder.DropColumn(
                name: "HWId",
                table: "Base");

            migrationBuilder.RenameColumn(
                name: "CollarId",
                table: "Collar",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "BaseId",
                table: "Base",
                newName: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class userMissingSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MissingSubscription",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MissingSubscription",
                table: "AspNetUsers");
        }
    }
}

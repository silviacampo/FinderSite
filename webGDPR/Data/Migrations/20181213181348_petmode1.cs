using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class petmode1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastModeId",
                table: "Pet",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pet_LastModeId",
                table: "Pet",
                column: "LastModeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pet_PetTrackingInfo_LastModeId",
                table: "Pet",
                column: "LastModeId",
                principalTable: "PetTrackingInfo",
                principalColumn: "PetTrackingInfoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pet_PetTrackingInfo_LastModeId",
                table: "Pet");

            migrationBuilder.DropIndex(
                name: "IX_Pet_LastModeId",
                table: "Pet");

            migrationBuilder.DropColumn(
                name: "LastModeId",
                table: "Pet");
        }
    }
}

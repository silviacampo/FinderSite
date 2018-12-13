using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class petmode3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pet_PetTrackingInfo_LastModeId",
                table: "Pet");

            migrationBuilder.AddForeignKey(
                name: "FK_Pet_PetMode_LastModeId",
                table: "Pet",
                column: "LastModeId",
                principalTable: "PetMode",
                principalColumn: "PetModeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pet_PetMode_LastModeId",
                table: "Pet");

            migrationBuilder.AddForeignKey(
                name: "FK_Pet_PetTrackingInfo_LastModeId",
                table: "Pet",
                column: "LastModeId",
                principalTable: "PetTrackingInfo",
                principalColumn: "PetTrackingInfoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

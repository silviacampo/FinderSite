using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class bannedDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Aproved",
                table: "Device",
                newName: "Banned");

            migrationBuilder.AlterColumn<string>(
                name: "PetId",
                table: "PetTrackingInfo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "PetTrackingInfo",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "PetTrackingInfo",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.CreateIndex(
                name: "IX_PetTrackingInfo_PetId",
                table: "PetTrackingInfo",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_PetTrackingInfo_Pet_PetId",
                table: "PetTrackingInfo",
                column: "PetId",
                principalTable: "Pet",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetTrackingInfo_Pet_PetId",
                table: "PetTrackingInfo");

            migrationBuilder.DropIndex(
                name: "IX_PetTrackingInfo_PetId",
                table: "PetTrackingInfo");

            migrationBuilder.RenameColumn(
                name: "Banned",
                table: "Device",
                newName: "Aproved");

            migrationBuilder.AlterColumn<string>(
                name: "PetId",
                table: "PetTrackingInfo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Longitude",
                table: "PetTrackingInfo",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<long>(
                name: "Latitude",
                table: "PetTrackingInfo",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}

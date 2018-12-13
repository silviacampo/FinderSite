using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class petmode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CollarId",
                table: "PetTrackingInfo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PetMode",
                columns: table => new
                {
                    PetModeId = table.Column<string>(nullable: false),
                    PetId = table.Column<string>(nullable: true),
                    CollarId = table.Column<string>(nullable: true),
                    BaseId = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetMode", x => x.PetModeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PetTrackingInfo_CollarId",
                table: "PetTrackingInfo",
                column: "CollarId");

            migrationBuilder.AddForeignKey(
                name: "FK_PetTrackingInfo_Collar_CollarId",
                table: "PetTrackingInfo",
                column: "CollarId",
                principalTable: "Collar",
                principalColumn: "CollarId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetTrackingInfo_Collar_CollarId",
                table: "PetTrackingInfo");

            migrationBuilder.DropTable(
                name: "PetMode");

            migrationBuilder.DropIndex(
                name: "IX_PetTrackingInfo_CollarId",
                table: "PetTrackingInfo");

            migrationBuilder.AlterColumn<string>(
                name: "CollarId",
                table: "PetTrackingInfo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

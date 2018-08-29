using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class pettrackingInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PetTrackingInfo",
                columns: table => new
                {
                    PetTrackingInfoId = table.Column<string>(nullable: false),
                    PetId = table.Column<string>(nullable: true),
                    CollarId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Latitude = table.Column<long>(nullable: false),
                    Longitude = table.Column<long>(nullable: false),
                    Acceleration = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetTrackingInfo", x => x.PetTrackingInfoId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PetTrackingInfo");
        }
    }
}

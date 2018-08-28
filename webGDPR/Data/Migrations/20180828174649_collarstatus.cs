using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class collarstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Battery",
                table: "Collar");

            migrationBuilder.DropColumn(
                name: "IsConnected",
                table: "Collar");

            migrationBuilder.DropColumn(
                name: "IsGPSConnected",
                table: "Collar");

            migrationBuilder.DropColumn(
                name: "Radio",
                table: "Collar");

            migrationBuilder.AddColumn<string>(
                name: "LastStatusId",
                table: "Collar",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastStatusId",
                table: "Base",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CollarStatus",
                columns: table => new
                {
                    CollarStatusId = table.Column<string>(nullable: false),
                    CollarId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsConnected = table.Column<bool>(nullable: false),
                    IsGPSConnected = table.Column<bool>(nullable: false),
                    Battery = table.Column<int>(nullable: false),
                    Radio = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollarStatus", x => x.CollarStatusId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collar_LastStatusId",
                table: "Collar",
                column: "LastStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Base_LastStatusId",
                table: "Base",
                column: "LastStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Base_BaseStatus_LastStatusId",
                table: "Base",
                column: "LastStatusId",
                principalTable: "BaseStatus",
                principalColumn: "BaseStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Collar_CollarStatus_LastStatusId",
                table: "Collar",
                column: "LastStatusId",
                principalTable: "CollarStatus",
                principalColumn: "CollarStatusId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Base_BaseStatus_LastStatusId",
                table: "Base");

            migrationBuilder.DropForeignKey(
                name: "FK_Collar_CollarStatus_LastStatusId",
                table: "Collar");

            migrationBuilder.DropTable(
                name: "CollarStatus");

            migrationBuilder.DropIndex(
                name: "IX_Collar_LastStatusId",
                table: "Collar");

            migrationBuilder.DropIndex(
                name: "IX_Base_LastStatusId",
                table: "Base");

            migrationBuilder.DropColumn(
                name: "LastStatusId",
                table: "Collar");

            migrationBuilder.AddColumn<int>(
                name: "Battery",
                table: "Collar",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsConnected",
                table: "Collar",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGPSConnected",
                table: "Collar",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Radio",
                table: "Collar",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "LastStatusId",
                table: "Base",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

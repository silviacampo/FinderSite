using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class petRefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PetId",
                table: "PetCollar",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CollarId",
                table: "PetCollar",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastCollarId",
                table: "Pet",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastTrackingInfoId",
                table: "Pet",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PetCollar_CollarId",
                table: "PetCollar",
                column: "CollarId");

            migrationBuilder.CreateIndex(
                name: "IX_PetCollar_PetId",
                table: "PetCollar",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Pet_LastCollarId",
                table: "Pet",
                column: "LastCollarId");

            migrationBuilder.CreateIndex(
                name: "IX_Pet_LastTrackingInfoId",
                table: "Pet",
                column: "LastTrackingInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pet_PetCollar_LastCollarId",
                table: "Pet",
                column: "LastCollarId",
                principalTable: "PetCollar",
                principalColumn: "PetCollarId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pet_PetTrackingInfo_LastTrackingInfoId",
                table: "Pet",
                column: "LastTrackingInfoId",
                principalTable: "PetTrackingInfo",
                principalColumn: "PetTrackingInfoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PetCollar_Collar_CollarId",
                table: "PetCollar",
                column: "CollarId",
                principalTable: "Collar",
                principalColumn: "CollarId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PetCollar_Pet_PetId",
                table: "PetCollar",
                column: "PetId",
                principalTable: "Pet",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pet_PetCollar_LastCollarId",
                table: "Pet");

            migrationBuilder.DropForeignKey(
                name: "FK_Pet_PetTrackingInfo_LastTrackingInfoId",
                table: "Pet");

            migrationBuilder.DropForeignKey(
                name: "FK_PetCollar_Collar_CollarId",
                table: "PetCollar");

            migrationBuilder.DropForeignKey(
                name: "FK_PetCollar_Pet_PetId",
                table: "PetCollar");

            migrationBuilder.DropIndex(
                name: "IX_PetCollar_CollarId",
                table: "PetCollar");

            migrationBuilder.DropIndex(
                name: "IX_PetCollar_PetId",
                table: "PetCollar");

            migrationBuilder.DropIndex(
                name: "IX_Pet_LastCollarId",
                table: "Pet");

            migrationBuilder.DropIndex(
                name: "IX_Pet_LastTrackingInfoId",
                table: "Pet");

            migrationBuilder.DropColumn(
                name: "LastCollarId",
                table: "Pet");

            migrationBuilder.DropColumn(
                name: "LastTrackingInfoId",
                table: "Pet");

            migrationBuilder.AlterColumn<string>(
                name: "PetId",
                table: "PetCollar",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CollarId",
                table: "PetCollar",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

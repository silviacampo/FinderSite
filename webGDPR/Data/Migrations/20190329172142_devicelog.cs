using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class devicelog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgeUnit",
                table: "Pet",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeigthUnit",
                table: "Pet",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "DeviceLog",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLog_DeviceId",
                table: "DeviceLog",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceLog_Device_DeviceId",
                table: "DeviceLog",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "DeviceId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceLog_Device_DeviceId",
                table: "DeviceLog");

            migrationBuilder.DropIndex(
                name: "IX_DeviceLog_DeviceId",
                table: "DeviceLog");

            migrationBuilder.DropColumn(
                name: "AgeUnit",
                table: "Pet");

            migrationBuilder.DropColumn(
                name: "WeigthUnit",
                table: "Pet");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "DeviceLog",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceLog_Device_DeviceId",
                table: "DeviceLog");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "DeviceLog",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceLog_Device_DeviceId",
                table: "DeviceLog",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "DeviceId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceLog_Device_DeviceId",
                table: "DeviceLog");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "DeviceLog",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceLog_Device_DeviceId",
                table: "DeviceLog",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "DeviceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class petfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Pet",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

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
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Collar",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HWId",
                table: "Collar",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Base",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HWId",
                table: "Base",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLog_DeviceId",
                table: "DeviceLog",
                column: "DeviceId");

        //    migrationBuilder.AddForeignKey(
        //        name: "FK_DeviceLog_Device_DeviceId",
        //        table: "DeviceLog",
        //        column: "DeviceId",
        //        principalTable: "Device",
        //        principalColumn: "DeviceId",
        //        onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_DeviceLog_Device_DeviceId",
            //    table: "DeviceLog");

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
                name: "Name",
                table: "Pet",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "DeviceLog",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Collar",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "HWId",
                table: "Collar",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Base",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "HWId",
                table: "Base",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}

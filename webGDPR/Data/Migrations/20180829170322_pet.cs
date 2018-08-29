using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class pet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ConnectedTo",
                table: "CollarStatus",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConnectedTo",
                table: "BaseStatus",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Pet",
                columns: table => new
                {
                    PetId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Breeding = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Age = table.Column<string>(nullable: true),
                    HealthComments = table.Column<string>(nullable: true),
                    ImageFileName = table.Column<string>(nullable: true),
                    PageFileName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pet", x => x.PetId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollarStatus_ConnectedTo",
                table: "CollarStatus",
                column: "ConnectedTo");

            migrationBuilder.CreateIndex(
                name: "IX_BaseStatus_ConnectedTo",
                table: "BaseStatus",
                column: "ConnectedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseStatus_Device_ConnectedTo",
                table: "BaseStatus",
                column: "ConnectedTo",
                principalTable: "Device",
                principalColumn: "DeviceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CollarStatus_Base_ConnectedTo",
                table: "CollarStatus",
                column: "ConnectedTo",
                principalTable: "Base",
                principalColumn: "BaseId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseStatus_Device_ConnectedTo",
                table: "BaseStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_CollarStatus_Base_ConnectedTo",
                table: "CollarStatus");

            migrationBuilder.DropTable(
                name: "Pet");

            migrationBuilder.DropIndex(
                name: "IX_CollarStatus_ConnectedTo",
                table: "CollarStatus");

            migrationBuilder.DropIndex(
                name: "IX_BaseStatus_ConnectedTo",
                table: "BaseStatus");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectedTo",
                table: "CollarStatus",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConnectedTo",
                table: "BaseStatus",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

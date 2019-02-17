using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class SubscriptionProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Width",
                table: "Product",
                nullable: true,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<float>(
                name: "Weight",
                table: "Product",
                nullable: true,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<int>(
                name: "QuantityInStock",
                table: "Product",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<float>(
                name: "Length",
                table: "Product",
                nullable: true,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<float>(
                name: "Height",
                table: "Product",
                nullable: true,
                oldClrType: typeof(float));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Width",
                table: "Product",
                nullable: false,
                oldClrType: typeof(float),
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Weight",
                table: "Product",
                nullable: false,
                oldClrType: typeof(float),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "QuantityInStock",
                table: "Product",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Length",
                table: "Product",
                nullable: false,
                oldClrType: typeof(float),
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Height",
                table: "Product",
                nullable: false,
                oldClrType: typeof(float),
                oldNullable: true);
        }
    }
}

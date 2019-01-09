using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
    public partial class productlenght : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Deep",
                table: "Product",
                newName: "Length");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingAddressId",
                table: "Invoice",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Invoice",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressId",
                table: "Invoice",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Invoice",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    AddressLine1 = table.Column<string>(nullable: true),
                    AddressLine2 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "Tax",
                columns: table => new
                {
                    TaxId = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Pourcentage = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax", x => x.TaxId);
                });

            migrationBuilder.CreateTable(
                name: "TaxAmount",
                columns: table => new
                {
                    TaxAmountId = table.Column<string>(nullable: false),
                    TaxId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Pourcentage = table.Column<float>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    InvoiceId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxAmount", x => x.TaxAmountId);
                    table.ForeignKey(
                        name: "FK_TaxAmount_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "InvoiceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaxAmount_Tax_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Tax",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_BillingAddressId",
                table: "Invoice",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ShippingAddressId",
                table: "Invoice",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxAmount_InvoiceId",
                table: "TaxAmount",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxAmount_TaxId",
                table: "TaxAmount",
                column: "TaxId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Address_BillingAddressId",
                table: "Invoice",
                column: "BillingAddressId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Address_ShippingAddressId",
                table: "Invoice",
                column: "ShippingAddressId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Address_BillingAddressId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Address_ShippingAddressId",
                table: "Invoice");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "TaxAmount");

            migrationBuilder.DropTable(
                name: "Tax");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_BillingAddressId",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_ShippingAddressId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "BillingAddressId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "ShippingAddressId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Invoice");

            migrationBuilder.RenameColumn(
                name: "Length",
                table: "Product",
                newName: "Deep");
        }
    }
}

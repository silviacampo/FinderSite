//this migration is not working automatically. but it's useful to keep the info in the project
using Microsoft.EntityFrameworkCore.Migrations;

namespace webGDPR.Data.Migrations
{
	public partial class ProductsInitialization : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
				migrationBuilder.InsertData(
				table: "product",
			   columns: new[] { "Model","Name","Text","Type","Price" },
			   values: new object[] { "S-1", "Subscription", "Monthly Subscription", "services", 10 });

				migrationBuilder.InsertData(
				table: "product",
				columns: new[] { "Model", "Name", "Text", "Type", "Price" },
				values: new object[] { "SD2-1", "SubscriptionDiscount2-1", "Subscription Discount for two trackers", "services", -3 });

				migrationBuilder.InsertData(
				table: "product",
				columns: new[] { "Model", "Name", "Text", "Type", "Price" },
				values: new object[] { "SD3+-1", "SubscriptionDiscount3+-1", "Subscription Discount for three or more trackers", "services", -5 });
		}
		}
	}

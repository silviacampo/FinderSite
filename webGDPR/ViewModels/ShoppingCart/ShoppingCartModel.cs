using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//https://github.com/smartstoreag/SmartStoreNET/tree/3.x/src/Presentation/SmartStore.Web

namespace webGDPR.ViewModels.ShoppingCart
{
	public partial class ShoppingCartModel
	{
		public ShoppingCartModel()
		{
			Items = new List<ShoppingCartItemModel>();
		}

		public IList<ShoppingCartItemModel> Items { get; set; }
		public int TotalProducts { get; set; }
		public string SubTotal { get; set; }
		public bool DisplayCheckoutButton { get; set; }
		public bool CurrentCustomerIsGuest { get; set; }
		public bool AnonymousCheckoutAllowed { get; set; }
		public bool ShowProductImages { get; set; }
		public int ThumbSize { get; set; }
		public bool DisplayMoveToWishlistButton { get; set; }

	}
}

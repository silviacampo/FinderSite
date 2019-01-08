using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.ViewModels.ShoppingCart
{
	public partial class OffCanvasCartModel
	{
		// product counts
		public int CartItemsCount { get; set; }
		public int WishlistItemsCount { get; set; }

		// settings
		public bool ShoppingCartEnabled { get; set; }
		public bool WishlistEnabled { get; set; }
	}
}

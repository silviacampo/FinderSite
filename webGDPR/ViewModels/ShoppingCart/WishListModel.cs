using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.ViewModels.ShoppingCart
{
	public partial class WishlistModel
	{
		public WishlistModel()
		{
			Items = new List<WishListItemModel>();
			Warnings = new List<string>();
		}

		public Guid CustomerGuid { get; set; }
		public string CustomerFullname { get; set; }

		public bool EmailWishlistEnabled { get; set; }

		public bool ShowSku { get; set; }

		public bool ShowProductImages { get; set; }

		public bool IsEditable { get; set; }

		public bool DisplayAddToCart { get; set; }

		public IList<WishListItemModel> Items { get; set; }

		public IList<string> Warnings { get; set; }

		public int ThumbSize { get; set; }
		public int BundleThumbSize { get; set; }
		public bool DisplayShortDesc { get; set; }
		public bool ShowProductBundleImages { get; set; }
		public bool ShowItemsFromWishlistToCartButton { get; set; }
	}
}

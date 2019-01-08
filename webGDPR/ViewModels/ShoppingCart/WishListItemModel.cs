using System.Collections.Generic;

namespace webGDPR.ViewModels.ShoppingCart
{
	public partial class WishListItemModel : ShoppingCartItemModel
	{
		public WishListItemModel() : base()
		{
			Warnings = new List<string>();
		}

		public bool VisibleIndividually { get; set; }

		public IList<string> Warnings { get; set; }

		public bool DisableBuyButton { get; set; }

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webGDPR.ViewModels.ShoppingCart
{
	public partial class ShoppingCartItemModel
	{
		public ShoppingCartItemModel()
		{
			Picture = new PictureModel();
			BundleItems = new List<ShoppingCartItemBundleItem>();
			AllowedQuantities = new List<SelectListItem>();
		}

		public int ProductId { get; set; }

		public string ProductName { get; set; }

		public string ShortDesc { get; set; }

		public string ProductSeName { get; set; }

		public string ProductUrl { get; set; }

		public int EnteredQuantity { get; set; }

		public string QuantityUnitName { get; set; }

		public List<SelectListItem> AllowedQuantities { get; set; }

		public int MinOrderAmount { get; set; }

		public int MaxOrderAmount { get; set; }

		public int QuantityStep { get; set; }

		//public QuantityControlType QuantiyControlType { get; set; }

		public string UnitPrice { get; set; }

		public string AttributeInfo { get; set; }

		public PictureModel Picture { get; set; }

		public IList<ShoppingCartItemBundleItem> BundleItems { get; set; }

		public DateTime CreatedOnUtc { get; set; }

	}

	public partial class ShoppingCartItemBundleItem
	{
		public string PictureUrl { get; set; }
		public string ProductName { get; set; }
		public string ProductSeName { get; set; }
		public string ProductUrl { get; set; }
	}
}

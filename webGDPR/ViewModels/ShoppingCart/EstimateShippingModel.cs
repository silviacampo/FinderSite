using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webGDPR.ViewModels.ShoppingCart
{
	public partial class EstimateShippingModel
	{
		public EstimateShippingModel()
		{
			ShippingOptions = new List<ShippingOptionModel>();
			Warnings = new List<string>();

			AvailableCountries = new List<SelectListItem>();
			AvailableStates = new List<SelectListItem>();
		}

		public bool Enabled { get; set; }

		public string ShippingInfoUrl { get; set; }

		public IList<ShippingOptionModel> ShippingOptions { get; set; }

		public IList<string> Warnings { get; set; }

		public int? CountryId { get; set; }
		public int? StateProvinceId { get; set; }
		public string ZipPostalCode { get; set; }

		public IList<SelectListItem> AvailableCountries { get; set; }
		public IList<SelectListItem> AvailableStates { get; set; }

		public partial class ShippingOptionModel
		{
			public int ShippingMethodId { get; set; }

			public string Name { get; set; }

			public string Description { get; set; }

			public string Price { get; set; }
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace webGDPR.ViewModels.ShoppingCart
{
	public partial class ButtonPaymentMethodModel
	{
		public ButtonPaymentMethodModel()
		{
			Items = new List<ButtonPaymentMethodItem>();
		}

		public IList<ButtonPaymentMethodItem> Items { get; set; }

		public partial class ButtonPaymentMethodItem
		{
			public string ActionName { get; set; }
			public string ControllerName { get; set; }
			public RouteValueDictionary RouteValues { get; set; }
		}
	}
}

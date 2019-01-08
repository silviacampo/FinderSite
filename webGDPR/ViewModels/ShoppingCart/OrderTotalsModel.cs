using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.ViewModels.ShoppingCart
{
	public partial class OrderTotalsModel
	{
		public OrderTotalsModel()
		{
			TaxRates = new List<TaxRate>();
			GiftCards = new List<GiftCard>();
		}

		public bool IsEditable { get; set; }

		public string SubTotal { get; set; }

		public string SubTotalDiscount { get; set; }
		public bool AllowRemovingSubTotalDiscount { get; set; }

		public string Shipping { get; set; }
		public bool RequiresShipping { get; set; }
		public string SelectedShippingMethod { get; set; }

		public string PaymentMethodAdditionalFee { get; set; }

		public string Tax { get; set; }
		public IList<TaxRate> TaxRates { get; set; }
		public bool DisplayTax { get; set; }
		public bool DisplayTaxRates { get; set; }
		public bool DisplayWeight { get; set; }

		public IList<GiftCard> GiftCards { get; set; }

		public string OrderTotalDiscount { get; set; }
		public bool AllowRemovingOrderTotalDiscount { get; set; }
		public int RedeemedRewardPoints { get; set; }
		public string RedeemedRewardPointsAmount { get; set; }
		public string CreditBalance { get; set; }
		public string OrderTotalRounding { get; set; }
		public string OrderTotal { get; set; }
		public decimal Weight { get; set; }
		public string WeightMeasureUnitName { get; set; }

		public bool ShowConfirmOrderLegalHint { get; set; }
		public string MinOrderSubtotalWarning { get; set; }

		public partial class TaxRate 
		{
			public string Rate { get; set; }
			public string Value { get; set; }
			public string Label { get; set; }
		}

		public partial class GiftCard 
		{
			public string CouponCode { get; set; }
			public string Amount { get; set; }
			public string Remaining { get; set; }
		}
	}
}

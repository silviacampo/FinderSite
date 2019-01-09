//https://stripe.com order.api +- 6% facturacion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Models
{
	public class Invoice
	{
		public string InvoiceId { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime DeliveryDate { get; set; }

		public decimal TotalOrder { get; set; }
		public List<TaxAmount> Taxes { get; set; }
		//public List<Payment> Payments { get; set; }
		//public ShippingInfo Shipping { get; set; }

		public string UserId { get; set; } //string.Empty is anonymous

		public string BillingAddressId { get; set; }
		public string ShippingAddressId { get; set; }

		public Address BillingAddress { get; set; }
		public Address ShippingAddress { get; set; }

		public bool Completed { get; set; }
		public bool IsWishList { get; set; }
		public bool Deleted { get; set; }

		public List<InvoiceItem> Items { get; set; }
	}
}

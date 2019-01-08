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
		//taxes
		//billing address
		//shipping address
		//payment methods


		public bool IsWishList { get; set; }
		public bool Deleted { get; set; }

		public List<InvoiceItem> Items { get; set; }
	}
}

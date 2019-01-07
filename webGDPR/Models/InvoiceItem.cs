using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Models
{
	public class InvoiceItem
	{
		public string InvoiceItemId { get; set; }
		public string InvoiceId { get; set; }
		public string ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public string Comment { get; set; }

		public DateTime CreationDate { get; set; }
	}
}

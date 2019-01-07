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


		public bool IsWishList { get; set; }
		public bool Deleted { get; set; }
	}
}

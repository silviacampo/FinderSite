using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Models
{
	public class Subscription
	{
		public string SubscriptionId { get; set; }

		public DateTime CreationDate { get; set; }

		public string ProductId { get; set; }

		[ForeignKey("ProductId")]
		public Product Product { get; set; }

		public string Note { get; set; }

		public string UserId { get; set; }

		public string LastPaymentId { get; set; }

		[ForeignKey("LastPaymentId")]
		public Payment LastPayment { get; set; }

		public DateTime ExpirationDate { get; set; }

		public bool Deleted { get; set; }
	}
}

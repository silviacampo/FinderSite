//https://www.taxjar.com/pricing/ expensive $17/month
namespace webGDPR.Models
{
	public class TaxAmount
	{
		public string TaxAmountId { get; set; }
		public string TaxId { get; set; }
		public Tax Tax { get; set; }
		public string Name { get; set; }
		public float Pourcentage { get; set; }
		public decimal Amount { get; set; }
	}
}
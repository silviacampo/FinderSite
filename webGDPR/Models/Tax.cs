//QC STATE	14.975%
//ON STATE	13.000%
namespace webGDPR.Models
{
	public class Tax
	{
		public string TaxId { get; set; }
		public string Country { get; set; }
		public string Province { get; set; }
		public string Name { get; set; }
		public float Pourcentage { get; set; }
	}
}
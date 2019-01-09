//https://smartystreets.com/products/single-address hast 250 validations /month free
//search free address validation
//if anonymous user add a captcha or first must enter the payment method
namespace webGDPR.Models
{
	public class Address
	{
		public string AddressId { get; set; }

		public string Country { get; set; }

		public string Province { get; set; }

		public string City { get; set; }

		public string PostalCode { get; set; }

		public string AddressLine1 { get; set; }

		public string AddressLine2 { get; set; }
	}
}
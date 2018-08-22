using System.ComponentModel.DataAnnotations;

namespace webGDPR.Models
{
	public class User
	{
		public string UserID { get; set; }

		// user ID from AspNetUser table.
		public string OwnerID { get; set; }

		public string Name { get; set; }

		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

	}
}
using System.Collections.Generic;
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

		public bool MissingSubscription { get; set; }

		public List<Base> Bases { get; set; }
		public List<Collar> Collars { get; set; }
		public List<Device> Devices { get; set; }

		public List<Pet> Pets { get; set; }
	}
}
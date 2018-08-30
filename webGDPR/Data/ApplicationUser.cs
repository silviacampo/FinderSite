using Microsoft.AspNetCore.Identity;

namespace webGDPR.Data
{
	public class ApplicationUser : IdentityUser
	{
		public string Name { get; set; }
		//public int Age { get; set; }
	}
}

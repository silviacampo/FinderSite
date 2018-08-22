using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webGDPR.Models;

namespace webGDPR.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{	}

		public DbSet<Post> Post { get; set; }
		public DbSet<User> User { get; set; }
	}
}

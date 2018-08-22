using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webGDPR.Models
{
	public class Post
	{
		public int PostId { get; set; }

		public string Title { get; set; }

		public string Body { get; set; }

		public string Images { get; set; }

		public string UserId { get; set; }

		//public User User { get; set; }
	}
}
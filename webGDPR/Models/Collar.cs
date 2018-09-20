using System.ComponentModel.DataAnnotations.Schema;
using webGDPR.Models;

namespace webGDPR.Models
{
	public class Collar
	{
		public const string InitialName = "Collar 1";
		public const int InitialNumber = 1;
		public const string InitialDescription = "";

		public string CollarId { get; set; }
		public string HWId { get; set; }
		public string Name { get; set; }
		public byte CollarNumber { get; set; }
		public byte BaseNumber { get; set; }

		public string Description { get; set; }

		public string UserId { get; set; }

		public string LastStatusId { get; set; }

		[ForeignKey("LastStatusId")]
		public CollarStatus LastStatus { get; set; }

		public bool Deleted { get; set; }
	}
}



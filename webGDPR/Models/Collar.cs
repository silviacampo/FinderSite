using System.ComponentModel.DataAnnotations.Schema;
using webGDPR.Models;

namespace AgendaSignalR.Infrastructure
{
	public class Collar
	{
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
	}
}



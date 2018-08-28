using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using webGDPR.Models;

namespace AgendaSignalR.Infrastructure
{
	public class Base
	{
		public string BaseId { get; set; }
		public byte BaseNumber { get; set; } //id by client

		public string HWId { get; set; }

		public string Name { get; set; }
		public string Text { get; set; }
		public string Description { get; set; }

		public string UserId { get; set; }

		public string LastStatusId { get; set; }

		[ForeignKey("LastStatusId")]
		public BaseStatus LastStatus { get; set; }

	}
}

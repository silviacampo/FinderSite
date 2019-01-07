using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using webGDPR.Models;

namespace webGDPR.Models
{
	public class Base
	{
		public const string InitialName = "Base 1";
		public const int InitialNumber = 1;
		public const string InitialDescription = "";

		public string BaseId { get; set; }
		public byte BaseNumber { get; set; } //id by client

		public string HWId { get; set; }

		public string Name { get; set; }
		public string Text { get; set; }
		public string Description { get; set; }

		public string UserId { get; set; }

		public DateTime CreationDate { get; set; }

		public string LastStatusId { get; set; }

		[ForeignKey("LastStatusId")]
		public BaseStatus LastStatus { get; set; }

		public bool Deleted { get; set; }

	}
}

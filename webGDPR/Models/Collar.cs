using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

		[DisplayName("Collar Code (ie FC123456)")]
		public string HWId { get; set; }
		[DisplayName("Displayed Name (Used in our web site and application)")]
		public string Name { get; set; }
		public byte CollarNumber { get; set; }
		public byte BaseNumber { get; set; }

		public string Description { get; set; }

		public string UserId { get; set; }

		public DateTime CreationDate { get; set; }

		public string LastStatusId { get; set; }

		[ForeignKey("LastStatusId")]
		public CollarStatus LastStatus { get; set; }

		public bool Deleted { get; set; }
	}
}



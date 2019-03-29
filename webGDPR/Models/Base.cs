using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

		[Required(ErrorMessage ="Please enter the Base Code")]
		[DisplayName("Base Code (ie FB654321)")]
		public string HWId { get; set; }

		[Required(ErrorMessage ="Please enter the Displayed Name")]
		[DisplayName("Displayed Name (Used in our web site and application)")]
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

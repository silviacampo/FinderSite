using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace webGDPR.Models
{
    public class PetCollar
    {
		public string PetCollarId { get; set; }
		public string PetId { get; set; }

		[JsonIgnore]
		[ForeignKey("PetId")]
		public Pet Pet { get; set; }

		public string CollarId { get; set; }

		[ForeignKey("CollarId")]
		public Collar Collar { get; set; }

		public string UserId { get; set; }

		public bool IsActive { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public DateTime CreationDate { get; set; }
	}
}

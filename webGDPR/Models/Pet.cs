using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Models
{
    public class Pet
    {
		public const string InitialName = "Pet 1";

		public string PetId { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Breeding { get; set; }
		public string Color { get; set; }
		public DateTime Birthdate { get; set; }
		public string Age { get; set; }
		public string Weigth { get; set; }
		public string HealthComments { get; set; }

		public string LastCollarId { get; set; }

		[ForeignKey("LastCollarId")]
		public PetCollar LastCollar { get; set; }

		public string LastTrackingInfoId { get; set; }

		[ForeignKey("LastTrackingInfoId")]
		public PetTrackingInfo LastTrackingInfo { get; set; }

		public string LastModeId { get; set; }

		[ForeignKey("LastModeId")]
		public PetMode LastMode { get; set; }

		public bool Deleted { get; set; }

		public string UserId { get; set; }
		public List<PetTrackingInfo> TrackingInfos { get; set; }

	}
}

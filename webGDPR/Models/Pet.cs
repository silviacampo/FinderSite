using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace webGDPR.Models
{
	public class Pet
    {
		public const string InitialName = "Pet 1";

		public string PetId { get; set; }

		[DisplayName("Pet Name (Used in our web site and application)")]
		public string Name { get; set; }
		public string Type { get; set; }
		[DisplayName("Breeding (if applicable)")]
		public string Breeding { get; set; }
		[DisplayName("Color/Pattern (if applicable ie: white, black, tabby, calico)")]
		public string Color { get; set; }
		public DateTime Birthdate { get; set; }
		public string Age { get; set; }
		[DisplayName(" ")]
		public string AgeUnit { get; set; }
		public string Gender { get; set; }
		public string Weigth { get; set; }
		[DisplayName(" ")]
		public string WeigthUnit { get; set; }
		[DisplayName("Pet Health Comments ")]
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

		[DisplayName("Level of Activity")]
		public ConfigModeTypes DefaultMode { get; set; } //int matching the enum PetModeTypes 2 a 6

		public bool Deleted { get; set; }

		public string UserId { get; set; }

		public DateTime CreationDate { get; set; }

		public List<PetTrackingInfo> TrackingInfos { get; set; }

	}
}

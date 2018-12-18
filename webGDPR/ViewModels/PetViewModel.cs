using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webGDPR.Infrastructure;
using webGDPR.Models;

namespace webGDPR.ViewModels
{
    public class PetViewModel
    {
		public string PetId { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Breeding { get; set; }
		public string Color { get; set; }
		public string Age { get; set; }
		public string HealthComments { get; set; }

		public string CollarName { get; set; }

		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public long Acceleration { get; set; }

		public bool IsMoving { get { return Acceleration > 0; } }

		public DateTime LocationCreationDate { get; set; }

		public string UserId { get; set; }

		public PaginatedList<PetTrackingInfo> PetTrackingInfos { get; internal set; }

		public string CurrentFilter { get; set; }

		public bool EmergencyOn { get; set; }

		public PaginatedList<PetMode> PetModes { get; internal set; }
		public string CurrentFilterMode { get; internal set; }

		public PetModeTypes DefaultMode { get; set; }
	}
}

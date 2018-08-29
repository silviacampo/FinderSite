using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

		public string ImageFileName { get; set; }
		public string PageFileName { get; set; }
		public string PageContent { get; set; }

		public string CollarName { get; set; }

		public long Latitude { get; set; }
		public long Longitude { get; set; }
		public long Acceleration { get; set; }

		public bool IsMoving { get { return Acceleration > 0; } }

		public DateTime LocationCreationDate { get; set; }

		public string UserId { get; set; }
	}
}

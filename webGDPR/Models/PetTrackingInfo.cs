using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Models
{
    public class PetTrackingInfo
    {
		public string PetTrackingInfoId { get; set; }
		public string PetId { get; set; }
		public string CollarId { get; set; }
		public string UserId { get; set; }

		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public long Acceleration { get; set; } 

		public bool IsMoving { get { return Acceleration > 0; } }

		public bool IsActive { get; set; }

		public DateTime CreationDate { get; set; }
	}
}

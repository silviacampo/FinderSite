using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.Models
{
	public class PetMode
	{
		public string PetModeId { get; set; }
		public string PetId { get; set; }
		public string CollarId { get; set; }

		public PetModeTypes Type { get; set; }

		public string UserId { get; set; }

		public bool IsActive { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public DateTime CreationDate { get; set; }
	}

	public enum PetModeTypes {
		Normal = 0,
		Emergency = 1,
		VeryActive = 2,
		Active = 3,
		Lazy = 4,
		VeryLazy = 5
	}
}

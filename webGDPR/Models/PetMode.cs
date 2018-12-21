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

		public ConfigModeTypes Type { get; set; }

		public string UserId { get; set; }

		public bool IsActive { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public DateTime CreationDate { get; set; }
	}

	public enum ConfigModeTypes {
		None = 0,
		Emergency = 1,
		Young = 2,
		Active = 3,
		Regular = 4,
		Lazy = 5,
		Mature = 6
	}
}

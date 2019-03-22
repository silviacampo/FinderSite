using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webGDPR.Models;

namespace webGDPR.ViewModels
{
	public class DashboardViewModel
	{
	}

	public class CollarPet
	{
		public string CollarId { get; set; }
		public string CollarHWId { get; set; }
		public string CollarName { get; set; }

		public string CollarLastStatusId { get; set; }

		[ForeignKey("CollarLastStatusId")]
		public CollarStatus CollarLastStatus { get; set; }

		public bool CollarDeleted { get; set; }

		public string PetId { get; set; }
		public string PetName { get; set; }
		public string PetType { get; set; }
		public string PetBreeding { get; set; }
		public string PetColor { get; set; }
		public DateTime PetBirthdate { get; set; }
		public string PetAge { get; set; }
		public string PetGender { get; set; }
		public string PetWeigth { get; set; }
		public string PetHealthComments { get; set; }

		public string PetLastTrackingInfoId { get; set; }

		[ForeignKey("PetLastTrackingInfoId")]
		public PetTrackingInfo PetLastTrackingInfo { get; set; }

		public string PetLastModeId { get; set; }

		[ForeignKey("LastModeId")]
		public PetMode PetLastMode { get; set; }

		public ConfigModeTypes PetDefaultMode { get; set; } //int matching the enum PetModeTypes 2 a 6

		public bool PetDeleted { get; set; }

	}
}

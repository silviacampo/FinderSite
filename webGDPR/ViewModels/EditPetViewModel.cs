using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webGDPR.Models
{
	public class EditPetViewModel
    {
		public const string InitialName = "Pet 1";

		public string PetId { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Breeding { get; set; }
		public string Color { get; set; }
		public string Age { get; set; }
		public string HealthComments { get; set; }

		public string ImageFileName { get; set; }
		public string PageFileName { get; set; }

		public string CollarId { get; set; }

		public IEnumerable<SelectListItem> Collars { get; set; }

		public string UserId { get; set; }
	}
}

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webGDPR.ViewModels
{
	public class EditCollarViewModel
    {
		public string CollarId { get; set; }
		public string HWId { get; set; }
		public string Name { get; set; }

		public string Description { get; set; }

		public string PetId { get; set; }

		public IEnumerable<SelectListItem> Pets { get; set; }

		public string UserId { get; set; }

		public bool Deleted { get; set; }
	}
}

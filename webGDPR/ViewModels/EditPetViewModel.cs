using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webGDPR.Models
{
	public class EditPetViewModel: Pet
    {
		public string ImageFileName { get; set; }
		public string PageFileName { get; set; }

		[DisplayName("Choose the collar that will use the pet")] 
		public string CollarId { get; set; }

		public IEnumerable<SelectListItem> Collars { get; set; }

		public IEnumerable<SelectListItem> Types { get; set; }

		public IEnumerable<SelectListItem> Breedings { get; set; }

		public IEnumerable<SelectListItem> Genders { get; set; }

		public IEnumerable<SelectListItem> AgeUnits { get; set; }

		public IEnumerable<SelectListItem> WeightUnits { get; set; }

		public IEnumerable<SelectListItem> Modes { get; set; }
	}
}

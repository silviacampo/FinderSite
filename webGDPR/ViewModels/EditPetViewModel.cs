using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webGDPR.Models
{
	public class EditPetViewModel: Pet
    {
		[DisplayName("Choose photos of your pet in case it's lost")]
		public string ImageFileName { get; set; }

		[DisplayName("Edit a page with information about your pet, can include photos previously uploaded.")]
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

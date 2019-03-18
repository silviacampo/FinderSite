using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webGDPR.Models
{
	public class EditPetViewModel
    {
		public string PetId { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Breeding { get; set; }
		public string Color { get; set; }
		public DateTime Birthdate { get; set; }
		public double Age { get; set; }
		public string AgeUnit { get; set; }
		public string Gender { get; set; }
		public double Weigth { get; set; }
		public string WeigthUnit { get; set; }
		public string HealthComments { get; set; }

		public string ImageFileName { get; set; }
		public string PageFileName { get; set; }

		public string CollarId { get; set; }

		public IEnumerable<SelectListItem> Collars { get; set; }

		public IEnumerable<SelectListItem> Types { get; set; }

		public IEnumerable<SelectListItem> Breedings { get; set; }

		public IEnumerable<SelectListItem> Genders { get; set; }

		public IEnumerable<SelectListItem> AgeUnits { get; set; }

		public IEnumerable<SelectListItem> WeightUnits { get; set; }

		public string UserId { get; set; }

		public ConfigModeTypes DefaultMode { get; set; }

		public IEnumerable<SelectListItem> Modes { get; set; }
	}
}

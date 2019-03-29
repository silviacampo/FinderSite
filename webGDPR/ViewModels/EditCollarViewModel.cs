using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using webGDPR.Models;

namespace webGDPR.ViewModels
{
	public class EditCollarViewModel: Collar
	{
		[DisplayName("Choose the pet that will use the collar")]
		public string PetId { get; set; }
		public IEnumerable<SelectListItem> Pets { get; set; }
	}
}

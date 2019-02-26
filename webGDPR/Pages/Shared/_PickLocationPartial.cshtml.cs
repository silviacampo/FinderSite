using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geocoding;
using Geocoding.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webGDPR.Data;
using webGDPR.Models;

namespace webGDPR.Pages.Shared
{
    public class _PickLocationPartialModel : PageModel
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;
		private readonly ILogger<_PickLocationPartialModel> _logger;

		public _PickLocationPartialModel(
			UserManager<ApplicationUser> userManager,
			ApplicationDbContext context,
			ILogger<_PickLocationPartialModel> logger)
		{
			_userManager = userManager;
			_context = context;
			_logger = logger;
		}

		[BindProperty]
		public User UserInfo { get; set; }

		public void OnGet()
        {

        }

		public async Task<IActionResult> OnPostAsync() {
			User user = await _context.User.FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));
			try
			{
				IGeocoder geocoder = new GoogleGeocoder() { ApiKey = "AIzaSyCTWrqkwFGLjbd3Xl3vAspkszIefneqFT4" };
				IEnumerable<Geocoding.Address> addresses = await geocoder.GeocodeAsync(UserInfo.FormattedAddress);
				user.Latitude = addresses.First().Coordinates.Latitude;
				user.Longitude = addresses.First().Coordinates.Longitude;
				user.FormattedAddress = addresses.First().FormattedAddress;
				_context.Update(user);
				await _context.SaveChangesAsync();
				/*
				 Interesting:

						 "geometry" : {
							"bounds" : {
							   "northeast" : {
								  "lat" : 40.7142522,
								  "lng" : -73.961247
							   },
							   "southwest" : {
								  "lat" : 40.7141632,
								  "lng" : -73.961376
							   }
							},
							"location" : {
							   "lat" : 40.7142015,
							   "lng" : -73.96130769999999
							},
							"location_type" : "ROOFTOP",
							"viewport" : {
							   "northeast" : {
								  "lat" : 40.7155566802915,
								  "lng" : -73.9599625197085
							   },
							   "southwest" : {
								  "lat" : 40.7128587197085,
								  "lng" : -73.9626604802915
							   }
							}
						 }

				 */
				//	}
				//}
			}
			catch (Exception ex)
			{
				var test = ex.Message;
			}
			return LocalRedirect("/Dashboard");
		}

	}
}
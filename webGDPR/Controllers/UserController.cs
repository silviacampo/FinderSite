using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Geocoding;
using Geocoding.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using webGDPR.Authorization;
using webGDPR.Data;
using webGDPR.Infrastructure;
using webGDPR.Infrastructure.CustomWebSockets;
using webGDPR.Models;
using webGDPR.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webGDPR.Controllers
{
	public class UserController : Controller
	{
		private readonly ApplicationDbContext _context;
		UserManager<ApplicationUser> _userManager;
		IMapper _mapper;
		ICustomWebSocketMessageHandler _webSocketMessageHandler;
		ICustomWebSocketFactory _wsFactory;

		public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, ICustomWebSocketMessageHandler webSocketMessageHandler, ICustomWebSocketFactory wsFactory)
		{
			_context = context;
			_userManager = userManager;
			_mapper = mapper;
			_webSocketMessageHandler = webSocketMessageHandler;
			_wsFactory = wsFactory;
		}

		// GET: /<controller>/
		[Authorize]
		public async Task<IActionResult> Index()
		{
			var contacts = from c in _context.User
						   select c;

			var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
							   User.IsInRole(Constants.ContactAdministratorsRole);

			var currentUserId = _userManager.GetUserId(User);

			// Only approved contacts are shown UNLESS you're authorized to see them
			// or you are the owner.
			if (!isAuthorized)
			{
				contacts = contacts.Where(c => c.OwnerID == currentUserId);
			}

			List<User> users = await contacts.ToListAsync();

			return View(users);
		}

		[Authorize]
		public async Task<IActionResult> Dashboard()
		{
			User user = await _context.User.Include(b => b.Bases).Include(c => c.Collars).Include(d => d.Devices).Include(d => d.Pets).FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));
			foreach (Collar c in user.Collars)
			{
				PetCollar petCollar = await _context.PetCollar.FirstOrDefaultAsync(p => p.CollarId == c.CollarId && p.IsActive);
				if (petCollar != null)
				{
					Pet p = _context.Pet.FirstOrDefault(pet => pet.PetId == petCollar.PetId && !pet.Deleted);
					c.Name = p.Name;
				}
			}
			return View(user);
		}

		[Authorize]
		public async Task<IActionResult> HWOverview()
		{
			HWOverviewViewModel model = new HWOverviewViewModel();

			User user = await _context.User.Include(b => b.Bases).Include(c => c.Collars).Include(d => d.Devices).Include(d => d.Pets).FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));
			foreach (Collar c in user.Collars)
			{
				PetCollar petCollar = await _context.PetCollar.FirstOrDefaultAsync(p => p.CollarId == c.CollarId && p.IsActive);
				if (petCollar != null)
				{
					Pet p = _context.Pet.FirstOrDefault(pet => pet.PetId == petCollar.PetId && !pet.Deleted);
					c.Name = p.Name;
				}
			}
			model.User = user;

			List<BaseStatus> BasesStatus = _context.BaseStatus.Where(s => s.UserId == user.UserID && s.CreationDate > DateTime.Now.AddDays(-7)).OrderBy(s => s.CreationDate).ToList();

			TimeSpan disconnectedMiliseconds = new TimeSpan(0);
			List<Tuple<string, TimeSpan>> connectedToMiliseconds = new List<Tuple<string, TimeSpan>>();
			foreach (Device d in user.Devices) {
				connectedToMiliseconds.Add(new Tuple<string, TimeSpan>(d.DeviceId, new TimeSpan(0)));
			}

			for (int i = 0; i < BasesStatus.Count - 1; i++)
			{
				TimeSpan x = BasesStatus[i + 1].CreationDate - BasesStatus[i].CreationDate;
				if (BasesStatus[i].IsConnected) {
						var y = connectedToMiliseconds.FirstOrDefault(d => d.Item1 == BasesStatus[i + 1].ConnectedTo);
						y.Item2.Add(x); 
				}
				else {
					//Disconnected time
					disconnectedMiliseconds.Add(x);
				}
			}
			string deviceId = connectedToMiliseconds.OrderByDescending(d => d.Item2.Ticks).First().Item1;

			//Most connected to X device

			//time charging batteries : has a battery that is charging, only if close to 24hs...

			//Radio strenth average

			List<CollarStatus> CollarsStatus = _context.CollarStatus.Where(s => s.UserId == user.UserID && s.CreationDate > DateTime.Now.AddDays(-7)).OrderBy(s => s.CreationDate).ToList();

			List<PetTrackingInfo> PetTrackingInfos = _context.PetTrackingInfo.Where(s => s.UserId == user.UserID && s.CreationDate > DateTime.Now.AddDays(-7)).OrderBy(s => s.CreationDate).ToList();

			//Disconnected time
			//Most connected to X base
			
			//gps Disconnected time
			//gps disconnedted rel to closed location in time

			//Battery level avg
			//Time with battery < 25

			//Radio strenth average
			//radio strenth rel to closed location in time

			double[] totalDistance = new double[24];

			for (int i = 0; i < PetTrackingInfos.Count - 1; i++)
			{

				var distance = DistanceCalculation.Calculate(PetTrackingInfos[i].Latitude, PetTrackingInfos[i].Longitude, PetTrackingInfos[i + 1].Latitude, PetTrackingInfos[i + 1].Longitude, 'K');
				int hour = PetTrackingInfos[i + 1].CreationDate.Hour;
				totalDistance[hour] += distance;
			}

			double totaldays = 0;
			if (PetTrackingInfos.Count > 0)
				totaldays = (PetTrackingInfos[PetTrackingInfos.Count - 1].CreationDate.Date - PetTrackingInfos[0].CreationDate.Date).TotalDays;

			model.AvgDistance = new double[24];
			for (int j = 0; j < 24; j++)
			{
				if (totaldays > 0)
					model.AvgDistance[j] = totalDistance[j] / totaldays;
				else
					model.AvgDistance[j] = totalDistance[j];
			}

			model.AvgDistanceDay = model.AvgDistance.Sum();

			model.MediumAvgDistance = new double[24];
			for (int j = 0; j < 24; j++)
			{
				model.MediumAvgDistance[j] = 1;
			}

			model.MediumAvgDistanceDay = model.MediumAvgDistance.Sum();

			model.PointVisited = new List<Tuple<PetTrackingInfo, string>>();
			foreach (PetTrackingInfo pti in PetTrackingInfos)
			{
				string color = string.Empty;
				if (pti.CreationDate.Hour < 2)
				{
					color = "purple";
				}
				else if (pti.CreationDate.Hour < 6)
				{
					color = "red";
				}
				else if (pti.CreationDate.Hour < 10)
				{
					color = "orange";
				}
				else if (pti.CreationDate.Hour < 14)
				{
					color = "yellow";
				}
				else if (pti.CreationDate.Hour < 18)
				{
					color = "green";
				}
				else if (pti.CreationDate.Hour < 22)
				{
					color = "blue";
				}
				else
				{
					color = "purple";
				}
				model.PointVisited.Add(new Tuple<PetTrackingInfo, string>(pti, color));
			}

			return View(model);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> PickLocation()
		{
			User user = await _context.User.FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));

			//user.FormattedAddress = "279 Bedford Ave, Brooklyn, NY 11211, US";
			//if (user.Latitude != 0 && user.Longitude != 0 && string.IsNullOrEmpty(user.FormattedAddress))
			//	using (var client = new HttpClient())
			//	{
			//		try
			//		{
			//			string url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={user.Latitude},{user.Longitude}&key=AIzaSyCTWrqkwFGLjbd3Xl3vAspkszIefneqFT4";
			//			using (var result = await client.GetAsync(url))
			//			{
			//				if (result.IsSuccessStatusCode)
			//				{
			//					string test1 = await result.Content.ReadAsStringAsync();
			//					string pattern = @"""formatted_address"" : "".*"",";
			//					var test2 = Regex.Match(test1, pattern, RegexOptions.IgnoreCase).Value;
			//					string[] test3 = test2.Split(":");
			//					user.FormattedAddress = test3[1].Substring(2, test3[1].Length - 4);
			//				}
			//			}
			//		}
			//		catch (Exception ex)
			//		{
			//			var test = ex.Message;
			//		}
			//	}

			return View(user);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> PickLocation(string FormattedAddress)
		{
			User user = await _context.User.FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));
			try
			{
				IGeocoder geocoder = new GoogleGeocoder() { ApiKey = "AIzaSyCTWrqkwFGLjbd3Xl3vAspkszIefneqFT4" };
				IEnumerable<Geocoding.Address> addresses = await geocoder.GeocodeAsync(FormattedAddress);
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
			return RedirectToAction("Dashboard");
		}

		[Authorize]
		public async Task<IActionResult> FixMissingSubscription()
		{
			User user = await _context.User.Include(c => c.Collars).Include(d => d.Subscriptions).ThenInclude(e => e.Product).FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));
			return View(user);
		}

		//
		// GET: /<controller>/Create
		[HttpGet]
		[Authorize]
		public IActionResult Create()
		{
			return View();
		}

		//
		// POST: /<controller>/Create

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Create(FormCollection collection)
		{
			try
			{
				string OwnerID = _userManager.GetUserId(User);
				string UserName = _userManager.GetUserName(User);

				ApplicationUser appUser = await _userManager.GetUserAsync(User);

				User u = new Models.User
				{
					Email = appUser.Email,
					Name = appUser.Name,
					OwnerID = appUser.Id
				};

				// requires using ContactManager.Authorization;
				//var isAuthorized = await AuthorizationService.AuthorizeAsync(
				//											User, u,
				//											UserOperations.Create);
				//if (!isAuthorized.Succeeded)
				//{
				//	return new ChallengeResult();
				//}

				_context.User.Add(u);
				await _context.SaveChangesAsync();

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		//
		// GET: /<controller>/Edit/5
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Edit(string id)
		{
			var Contact = await _context.User.FirstOrDefaultAsync(m => m.UserID == id);

			if (Contact == null)
			{
				return NotFound();
			}

			//var isAuthorized = await AuthorizationService.AuthorizeAsync(
			//										  User, Contact,
			//										  UserOperations.Update);
			//if (!isAuthorized.Succeeded)
			//{
			//	return new ChallengeResult();
			//}

			return View();
		}

		//
		// POST: /<controller>/Edit/5

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Edit(string id, User user)
		{
			try
			{
				// Fetch Contact from DB to get OwnerID.
				var contact = await _context
					.User.AsNoTracking()
					.FirstOrDefaultAsync(m => m.UserID == id);

				if (contact == null)
				{
					return NotFound();
				}

				//var isAuthorized = await AuthorizationService.AuthorizeAsync(
				//										 User, contact,
				//										 UserOperations.Update);
				//if (!isAuthorized.Succeeded)
				//{
				//	return new ChallengeResult();
				//}

				user.OwnerID = contact.OwnerID;

				_context.Attach(user).State = EntityState.Modified;

				await _context.SaveChangesAsync();

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		//
		// GET: /<controller>/Delete/5
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Delete(string id)
		{
			var Contact = await _context.User.FirstOrDefaultAsync(m => m.UserID == id);

			if (Contact == null)
			{
				return NotFound();
			}

			//var isAuthorized = await AuthorizationService.AuthorizeAsync(
			//										  User, Contact,
			//										  UserOperations.Delete);
			//if (!isAuthorized.Succeeded)
			//{
			//	return new ChallengeResult();
			//}

			return View();
		}

		//
		// POST: /<controller>/Delete/5

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Delete(string id, User user)
		{
			try
			{
				// Fetch Contact from DB to get OwnerID.
				var contact = await _context
					.User.FirstOrDefaultAsync(m => m.UserID == id);

				if (contact == null)
				{
					return NotFound();
				}

				//var isAuthorized = await AuthorizationService.AuthorizeAsync(
				//										 User, contact,
				//										 UserOperations.Delete);
				//if (!isAuthorized.Succeeded)
				//{
				//	return new ChallengeResult();
				//}

				_context.User.Remove(contact);
				await _context.SaveChangesAsync();

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

	}
}

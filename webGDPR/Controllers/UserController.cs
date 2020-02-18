using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Geocoding;
using Geocoding.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using webGDPR.Authorization;
using webGDPR.Data;
using webGDPR.Infrastructure.CustomWebSockets;
using webGDPR.Models;
using webGDPR.Models.Helper;
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
		private readonly ILogger<UserController> _logger;
		private readonly IHtmlLocalizer<UserController> _localizer;

		public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, ICustomWebSocketMessageHandler webSocketMessageHandler, ICustomWebSocketFactory wsFactory, ILogger<UserController> logger, IHtmlLocalizer<UserController> localizer)
		{
			_context = context;
			_userManager = userManager;
			_mapper = mapper;
			_webSocketMessageHandler = webSocketMessageHandler;
			_wsFactory = wsFactory;
			_logger = logger;
			_localizer = localizer;
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
			User user = await _context.User.Include(b => b.Bases).ThenInclude(b => b.LastStatus).ThenInclude(c => c.DeviceConnectedTo).Include(c => c.Collars).ThenInclude(b => b.LastStatus).ThenInclude(c => c.BaseConnectedTo).Include(d => d.Devices).Include(d => d.Pets).ThenInclude(pe=>pe.LastMode).Include(d => d.Pets).ThenInclude(pe => pe.LastCollar).Include(d => d.Pets).ThenInclude(pe => pe.LastTrackingInfo).FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));

			ViewData["WebSockets"] = _wsFactory.Group(user.Name);

			return View(user);
		}

		[Authorize]
		public async Task<IActionResult> ConnectionTimeline() {
			List<Infrastructure.TimelineItem> list = new List<Infrastructure.TimelineItem>();
			User user = await _context.User.Include(b => b.Bases).ThenInclude(b => b.LastStatus).ThenInclude(c => c.DeviceConnectedTo).Include(c => c.Collars).ThenInclude(b => b.LastStatus).ThenInclude(c => c.BaseConnectedTo).Include(d => d.Devices).Include(d => d.Pets).ThenInclude(pe => pe.LastMode).Include(d => d.Pets).ThenInclude(pe => pe.LastCollar).Include(d => d.Pets).ThenInclude(pe => pe.LastTrackingInfo).FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));
			List<string> devicesID = user.Devices.Select(l=>l.DeviceId).ToList();
			List<DeviceLog> logs = await _context.DeviceLog.Where(l=> devicesID.Contains(l.DeviceId)).OrderByDescending(l=>l.CreationDate).Take(10).ToListAsync();
			foreach (var log in logs) {
				list.Add(new Infrastructure.TimelineItem() {
					ItemDate = log.CreationDate,
					ItemLeftTitle = user.Devices.FirstOrDefault(l => l.DeviceId == log.DeviceId).GetName,
					ItemMessage = log.Reason,
					ItemMore = log.Message,
					Orientation = (log.Reason.Contains("WebSocket") ? Infrastructure.TimelineItemOrientation.left: Infrastructure.TimelineItemOrientation.right)

				});
			}
			return View(list);
		}

		[Authorize]
		public async Task<IActionResult> ConnectionTimelineMore(int page)
		{
			List<Infrastructure.TimelineItem> list = new List<Infrastructure.TimelineItem>();
			User user = await _context.User.Include(b => b.Bases).ThenInclude(b => b.LastStatus).ThenInclude(c => c.DeviceConnectedTo).Include(c => c.Collars).ThenInclude(b => b.LastStatus).ThenInclude(c => c.BaseConnectedTo).Include(d => d.Devices).Include(d => d.Pets).ThenInclude(pe => pe.LastMode).Include(d => d.Pets).ThenInclude(pe => pe.LastCollar).Include(d => d.Pets).ThenInclude(pe => pe.LastTrackingInfo).FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));
			List<string> devicesID = user.Devices.Select(l => l.DeviceId).ToList();
			List<DeviceLog> logs = await _context.DeviceLog.Where(l => devicesID.Contains(l.DeviceId)).OrderByDescending(l => l.CreationDate).Skip(page * 10).Take(10).ToListAsync();
			foreach (var log in logs)
			{
				list.Add(new Infrastructure.TimelineItem()
				{
					ItemDate = log.CreationDate,
					ItemLeftTitle = user.Devices.FirstOrDefault(l => l.DeviceId == log.DeviceId).GetName,
					ItemMessage = log.Reason,
					ItemMore = log.Message,
					Orientation = (log.Reason.Contains("WebSocket") ? Infrastructure.TimelineItemOrientation.left : Infrastructure.TimelineItemOrientation.right)

				});
			}
			return new JsonResult(list);
		}

		[Authorize]
		public async Task<IActionResult> HWOverview()
		{
			HWOverviewViewModel model = new HWOverviewViewModel();

			await CalculateHWStatsAsync(model);

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> HWPeriod(string period = "W")
		{
			HWOverviewViewModel model = new HWOverviewViewModel();

			await CalculateHWStatsAsync(model, period);

			return PartialView("_HWStatsPartial", model);
		}

		private async Task CalculateHWStatsAsync(HWOverviewViewModel model, string period = "W") {
			DateTime limitDateTime;
			TimeSpan periodTimeSpan;
			if (period == "W")
			{
				limitDateTime = DateTime.Now.AddDays(-7);
				periodTimeSpan = new TimeSpan(7, 0, 0, 0);
			}
			else if (period == "M")
			{
				limitDateTime = DateTime.Now.AddMonths(-1);
				periodTimeSpan = new TimeSpan(Convert.ToInt32((DateTime.Now - limitDateTime).TotalDays), 0, 0, 0);
			}
			else
			{
				limitDateTime = DateTime.Now.AddMonths(-6);
				periodTimeSpan = new TimeSpan(Convert.ToInt32((DateTime.Now - limitDateTime).TotalDays), 0, 0, 0);
			}

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

			model.DeviceStats = new List<DeviceStats>();
			foreach (Device d in user.Devices)
			{
				if (!d.Banned)
					model.DeviceStats.Add(new DeviceStats() { Device = d });
			}

			model.BaseStats = new List<BaseStats>();
			foreach (Base b in user.Bases)
			{
				if (!b.Deleted)
				model.BaseStats.Add(new BaseStats() { Base = b });
			}

			model.CollarStats = new List<CollarStats>();
			foreach (Collar c in user.Collars)
			{
				if (!c.Deleted)
				model.CollarStats.Add(new CollarStats() { Collar = c });
			}

			List<Tuple<string, TimeSpan>> SumConnectedToDeviceTimeSpan = new List<Tuple<string, TimeSpan>>();
			foreach (Device d in user.Devices)
			{
				SumConnectedToDeviceTimeSpan.Add(new Tuple<string, TimeSpan>(d.DeviceId, new TimeSpan(0)));
			}

			List<Tuple<string, TimeSpan>> SumConnectedToBaseTimeSpan = new List<Tuple<string, TimeSpan>>();
			foreach (Base b in user.Bases)
			{
				SumConnectedToBaseTimeSpan.Add(new Tuple<string, TimeSpan>(b.BaseId, new TimeSpan(0)));
			}

			foreach (DeviceStats ds in model.DeviceStats)
			{
				List<DeviceLog> DeviceLog = _context.DeviceLog.Where(s => s.DeviceId == ds.Device.DeviceId && (s.Reason == "WebSocket Remove" || s.Reason == "WebSocket Add") && s.CreationDate > limitDateTime).OrderBy(s => s.CreationDate).ToList();

				if (DeviceLog.Count() > 0)
				{
					periodTimeSpan = new TimeSpan(Convert.ToInt32((DeviceLog.Last().CreationDate - DeviceLog.First().CreationDate).TotalDays), 0, 0, 0);
				}
				else
				{
					periodTimeSpan = new TimeSpan(0);
				}

				ds.ConnectedTimeSpan = new TimeSpan(0);

				for (int i = 0; i < DeviceLog.Count - 1; i++)
				{
					TimeSpan x = DeviceLog[i + 1].CreationDate - DeviceLog[i].CreationDate;
					if (DeviceLog[i].Reason == "WebSocket Add") {
						ds.ConnectedTimeSpan = ds.ConnectedTimeSpan.Add(x);
					}
				}
				if (periodTimeSpan.TotalMinutes > 0)
				{
					ds.AvgConnected = Math.Round(ds.ConnectedTimeSpan.TotalHours * 24 / periodTimeSpan.TotalHours, 2);
				}
				else
				{
					ds.AvgConnected = 0;
				}
			}

			foreach (BaseStats bs in model.BaseStats)
			{
				List<BaseStatus> BasesStatus = _context.BaseStatus.Where(s => s.BaseId == bs.Base.BaseId && s.CreationDate > limitDateTime).OrderBy(s => s.CreationDate).ToList();

				if (BasesStatus.Count() > 0)
				{
					periodTimeSpan = new TimeSpan(Convert.ToInt32((BasesStatus.Last().CreationDate - BasesStatus.First().CreationDate).TotalDays), 0, 0, 0);
				}
				else {
					periodTimeSpan = new TimeSpan(0);
				}
				bs.DisconnectedTimeSpan = new TimeSpan(0);
				bs.ConnectedToTimeSpan = new List<Tuple<string, TimeSpan>>();
				foreach (Device d in user.Devices)
				{
					bs.ConnectedToTimeSpan.Add(new Tuple<string, TimeSpan>(d.DeviceId, new TimeSpan(0)));
				}
				bs.PluginTimSpan = new TimeSpan(0);
				bs.RadioTimeSpan = new TimeSpan[101];
				bs.IsChargingTimeSpan = new TimeSpan(0);
				bs.BatteriesChargingMore75percent = false;

				for (int i = 0; i < BasesStatus.Count - 1; i++)
				{
					TimeSpan x = BasesStatus[i + 1].CreationDate - BasesStatus[i].CreationDate;
					if (BasesStatus[i].IsConnected)
					{
						if (user.Devices.Count > 0)
						{
							//Time connected to X device
							Tuple<string, TimeSpan> t = bs.ConnectedToTimeSpan.FirstOrDefault(d => d.Item1 == BasesStatus[i].ConnectedTo);
							TimeSpan ts = t.Item2;
							ts = ts.Add(x);
							bs.ConnectedToTimeSpan.Remove(t);
							bs.ConnectedToTimeSpan.Add(Tuple.Create(t.Item1, ts));

							Tuple<string, TimeSpan> st = SumConnectedToDeviceTimeSpan.FirstOrDefault(d => d.Item1 == BasesStatus[i].ConnectedTo);
							TimeSpan sts = st.Item2;
							sts = sts.Add(x);
							SumConnectedToDeviceTimeSpan.Remove(st);
							SumConnectedToDeviceTimeSpan.Add(Tuple.Create(st.Item1, sts));
						}
					}
					else
					{
						//Disconnected time
						bs.DisconnectedTimeSpan = bs.DisconnectedTimeSpan.Add(x);
					}

					if (BasesStatus[i].IsPlugged) {
						//time plugin
						bs.PluginTimSpan = bs.PluginTimSpan.Add(x);
					}

					//time per Radio strenth
					if (bs.RadioTimeSpan[BasesStatus[i].Radio] == null)
					{
						bs.RadioTimeSpan[BasesStatus[i].Radio] = new TimeSpan(0);
					}
					bs.RadioTimeSpan[BasesStatus[i].Radio] = bs.RadioTimeSpan[BasesStatus[i].Radio].Add(x);

					//time charging batteries
					if (BasesStatus[i].IsCharging)
					{
						bs.IsChargingTimeSpan = bs.IsChargingTimeSpan.Add(x);
					}
				}

				//Connected average
				if (periodTimeSpan.TotalMinutes > 0)
				{
					bs.AvgConnected = Math.Round((periodTimeSpan - bs.DisconnectedTimeSpan).TotalMinutes * 100 / periodTimeSpan.TotalMinutes, 2);
				}
				else {
					bs.AvgConnected = 0;
				}

				//Plugin average
				if (periodTimeSpan.TotalMinutes > 0)
				{
					bs.AvgPlugIn = Math.Round(bs.PluginTimSpan.TotalMinutes * 100 / periodTimeSpan.TotalMinutes, 2);
				}
				else {
					bs.AvgPlugIn = 0;
				}

				//Radio strenth average
				if (periodTimeSpan.TotalMinutes > 0)
				{
					double totalRadio = 0;
					for (int j = 0; j < bs.RadioTimeSpan.Count(); j++)
					{
						totalRadio = totalRadio + bs.RadioTimeSpan[j].TotalMinutes * j;
					}
					bs.AvgRadio = Math.Round(totalRadio / periodTimeSpan.TotalMinutes,2);
				}
				else {
					bs.AvgRadio = 0;
				}
				//time charging batteries : has a battery that is charging, only if close to 24hs...
				bs.BatteriesChargingMore75percent = (7 * 3 / 4) - (bs.IsChargingTimeSpan.TotalDays * 3 / 4) <= 0;
			}

			//Device is most connected to
			if (user.Devices.Count > 0)
			{
				model.MostConnectedToDevice = user.Devices.FirstOrDefault(c => c.DeviceId == SumConnectedToDeviceTimeSpan.OrderByDescending(d => d.Item2.Ticks).First().Item1);
			}

			model.PointsServiceLevel = new List<PointServiceLevel>();

			foreach (CollarStats cs in model.CollarStats)
			{
				List<CollarStatus> CollarsStatus = _context.CollarStatus.Where(s => s.CollarId == cs.Collar.CollarId && s.CreationDate > limitDateTime).OrderBy(s => s.CreationDate).ToList();

				List<PetTrackingInfo> PetTrackingInfos = _context.PetTrackingInfo.Where(s => s.CollarId == cs.Collar.CollarId && s.CreationDate > limitDateTime).OrderBy(s => s.CreationDate).ToList();

				if (CollarsStatus.Count() > 0)
				{
					periodTimeSpan = new TimeSpan(Convert.ToInt32((CollarsStatus.Last().CreationDate - CollarsStatus.First().CreationDate).TotalDays), 0, 0, 0);
				}
				else
				{
					periodTimeSpan = new TimeSpan(0);
				}

				cs.DisconnectedTimeSpan = new TimeSpan(0);
				cs.ConnectedToTimeSpan = new List<Tuple<string, TimeSpan>>();
				foreach (Base b in user.Bases)
				{
					cs.ConnectedToTimeSpan.Add(new Tuple<string, TimeSpan>(b.BaseId, new TimeSpan(0)));
				}

				cs.GPSDisconnectedTimeSpan = new TimeSpan(0);

				cs.RadioTimeSpan = new TimeSpan[101];

				cs.BatteryTimeSpan = new TimeSpan[101];

				for (int i = 0; i < CollarsStatus.Count - 1; i++)
				{
					TimeSpan x = CollarsStatus[i + 1].CreationDate - CollarsStatus[i].CreationDate;
					if (CollarsStatus[i].IsConnected)
					{
						if (user.Bases.Count > 0)
						{
							//Time connected to X base
							Tuple<string, TimeSpan> t = cs.ConnectedToTimeSpan.FirstOrDefault(d => d.Item1 == CollarsStatus[i].ConnectedTo);
							TimeSpan ts = t.Item2;
							ts = ts.Add(x);
							cs.ConnectedToTimeSpan.Remove(t);
							cs.ConnectedToTimeSpan.Add(Tuple.Create(t.Item1, ts));

							Tuple<string, TimeSpan> st = SumConnectedToBaseTimeSpan.FirstOrDefault(d => d.Item1 == CollarsStatus[i].ConnectedTo);
							TimeSpan sts = st.Item2;
							sts = sts.Add(x);
							SumConnectedToBaseTimeSpan.Remove(st);
							SumConnectedToBaseTimeSpan.Add(Tuple.Create(st.Item1, sts));

						}
					}
					else
					{
						//Disconnected time
						cs.DisconnectedTimeSpan = cs.DisconnectedTimeSpan.Add(x);
					}
					if (!CollarsStatus[i].IsGPSConnected)
					{
						//Disconnected time
						cs.GPSDisconnectedTimeSpan = cs.GPSDisconnectedTimeSpan.Add(x);
					}
					//time per Radio strenth
					if (cs.RadioTimeSpan[CollarsStatus[i].Radio] == null)
					{
						cs.RadioTimeSpan[CollarsStatus[i].Radio] = new TimeSpan(0);
					}
					cs.RadioTimeSpan[CollarsStatus[i].Radio] = cs.RadioTimeSpan[CollarsStatus[i].Radio].Add(x);

					if (!CollarsStatus[i].IsGPSConnected || CollarsStatus[i].Radio < 50)
					{
						var lastTracking = PetTrackingInfos.Where(p => p.CreationDate < CollarsStatus[i].CreationDate).OrderByDescending(p => p.CreationDate).FirstOrDefault();
                        if (lastTracking != null)
						    model.PointsServiceLevel.Add(new PointServiceLevel()
						    {
							    Radio = CollarsStatus[i].Radio,
							    GPS = CollarsStatus[i].IsGPSConnected,
							    Latitude = lastTracking.Latitude,
							    Longitude = lastTracking.Longitude
						    });
					}

					//time per Battery charge
					if (cs.BatteryTimeSpan[CollarsStatus[i].Battery] == null)
					{
						cs.BatteryTimeSpan[CollarsStatus[i].Battery] = new TimeSpan(0);
					}
					cs.BatteryTimeSpan[CollarsStatus[i].Battery] = cs.BatteryTimeSpan[CollarsStatus[i].Battery].Add(x);
				}

				//Connected average
				if (periodTimeSpan.TotalMinutes > 0)
				{
					cs.AvgConnected = Math.Round((periodTimeSpan - cs.DisconnectedTimeSpan).TotalMinutes * 100 / periodTimeSpan.TotalMinutes, 2);
				}
				else
				{
					cs.AvgConnected = 0;
				}

				//Plugin average
				if (periodTimeSpan.TotalMinutes > 0)
				{
					cs.AvgGPSConnected = Math.Round((periodTimeSpan - cs.GPSDisconnectedTimeSpan).TotalMinutes * 100 / periodTimeSpan.TotalMinutes, 2);
				}
				else
				{
					cs.AvgGPSConnected = 0;
				}

				//Radio strenth average
				if (periodTimeSpan.TotalMinutes > 0)
				{
					double totalRadio = 0;
					for (int j = 0; j < cs.RadioTimeSpan.Count(); j++)
					{
						totalRadio = totalRadio + cs.RadioTimeSpan[j].TotalMinutes * j;
					}
					cs.AvgRadio = Math.Round(totalRadio / periodTimeSpan.TotalMinutes, 2);
				}
				else
				{
					cs.AvgRadio = 0;
				}

				//Battery power average
				if (periodTimeSpan.TotalMinutes > 0)
				{
					double totalBattery = 0;
					for (int j = 0; j < cs.BatteryTimeSpan.Count(); j++)
					{
						totalBattery = totalBattery + cs.BatteryTimeSpan[j].TotalMinutes * j;
					}
					cs.AvgBattery = Math.Round(totalBattery / periodTimeSpan.TotalMinutes, 2);
				}
				else
				{
					cs.AvgBattery = 0;
				}

				//time charging batteries : has a battery that is charging, only if close to 24hs...
				TimeSpan[] BatteryMinus25 = cs.BatteryTimeSpan.Take(25).ToArray();
				cs.BatteryMinus25Minutes = BatteryMinus25.Sum(r => r.TotalSeconds) / 60;
			}

			//Base is most connected to
			if (user.Bases.Count > 0)
			{
				model.MostConnectedToBase = user.Bases.FirstOrDefault(c => c.BaseId == SumConnectedToBaseTimeSpan.OrderByDescending(d => d.Item2.Ticks).First().Item1);
			}
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
			
			try
			{
                User user = await _context.User.FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));

                IGeocoder geocoder = new GoogleGeocoder() { ApiKey = "AIzaSyCTWrqkwFGLjbd3Xl3vAspkszIefneqFT4" };
				IEnumerable<Geocoding.Address> addresses = await geocoder.GeocodeAsync(FormattedAddress);
				user.Latitude = addresses.First().Coordinates.Latitude;
				user.Longitude = addresses.First().Coordinates.Longitude;
				user.FormattedAddress = addresses.First().FormattedAddress;

				using (var client = new HttpClient())
				{
					try
					{
						string url = $"https://maps.googleapis.com/maps/api/timezone/json?location={user.Latitude},{user.Longitude}&timestamp={new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}&key=AIzaSyCTWrqkwFGLjbd3Xl3vAspkszIefneqFT4";
						using (var result = await client.GetAsync(url))
						{
							if (result.IsSuccessStatusCode)
							{
								TimeZoneAPIResponse timezone = JsonConvert.DeserializeObject<TimeZoneAPIResponse>(await result.Content.ReadAsStringAsync());
								user.TimeZoneId = timezone.timeZoneId;
								user.Offset = timezone.rawOffset / (60 * 60);
							}
						}
					}
					catch (Exception ex)
					{
						var test = ex.Message;
					}
				}


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

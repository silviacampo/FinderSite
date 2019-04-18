using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using AutoMapper;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using webGDPR.Data;
using webGDPR.Infrastructure;
using webGDPR.Infrastructure.CustomWebSockets;
using webGDPR.Models;
using webGDPR.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using System.Net.WebSockets;
using System.Threading;

namespace webGDPR.Controllers
{
    //[ServiceFilter(typeof(HostFilter))]
    [Authorize]
    public class MonitoringController : Controller
    {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly ApplicationDbContext _context;
		UserManager<ApplicationUser> _userManager;
		private readonly IHostingEnvironment _hostingEnvironment;
		IMapper _mapper;
		ICustomWebSocketMessageHandler _webSocketMessageHandler;
		ICustomWebSocketFactory _wsFactory;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IStringLocalizer<MonitoringController> _localizer;

		public MonitoringController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, IHostingEnvironment hostingEnvironment, ICustomWebSocketMessageHandler webSocketMessageHandler, ICustomWebSocketFactory wsFactory, SignInManager<ApplicationUser> signInManager, IStringLocalizer<MonitoringController> localizer)
		{
			_context = context;
			_userManager = userManager;
			_mapper = mapper;
			_hostingEnvironment = hostingEnvironment;
			_webSocketMessageHandler = webSocketMessageHandler;
			_wsFactory = wsFactory;
			_signInManager = signInManager;
			_localizer = localizer;
		}

		public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageIndex)
		{
			MonitoringViewModel vm = new MonitoringViewModel
			{
				WebSockets = _wsFactory.All(),
				Devices = _context.Device.ToList()
			};
			if (searchString != null)
			{
				pageIndex = 1;
			}
			else {
				if (currentFilter == null)
				{
					searchString = string.Empty;
				}
				else
				{
					searchString = currentFilter;
				}
			}
			vm.CurrentFilter = searchString;
			int pageSize = 50;
			vm.DeviceLogs = await PaginatedList<DeviceLog>.CreateAsync(
				_context.DeviceLog.Where(s => s.DeviceId.Contains(searchString) || s.Reason.Contains(searchString) || s.Message.Contains(searchString)).OrderByDescending(d => d.CreationDate).AsNoTracking(), pageIndex ?? 1, pageSize);
			return View(vm);
        }

		public async Task<IActionResult> Users(string searchString, string currentFilter, int? pageIndex)
		{
			MonitoringUsersViewModel vm = new MonitoringUsersViewModel();

			if (searchString != null)
			{
				pageIndex = 1;
			}
			else
			{
				if (currentFilter == null)
				{
					searchString = string.Empty;
				}
				else
				{
					searchString = currentFilter;
				}
			}
			vm.CurrentFilter = searchString;
			int pageSize = 50;
			vm.Users = await PaginatedList<User>.CreateAsync(
				_context.User.Where(s => s.UserID.Contains(searchString) || s.Name.Contains(searchString)).OrderBy(d => d.Name).AsNoTracking(), pageIndex ?? 1, pageSize);
			return View(vm);
		}

		public async Task<IActionResult> SendCloseConnection(Guid guid)
		{
			CustomWebSocket userWebSocket = _wsFactory.Client(guid);
			try
			{
				await userWebSocket.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
			}
			catch (Exception e)
			{
				log.Error("MonitoringController - SendCloseConnection: " + e.Message);
			}
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> MissingSubscriptionOn(string id)
		{
			bool result = await SetMissingSubscriptionAsync(true, id);
			if (!result)
			{
				return NotFound();
			}
			return RedirectToAction(nameof(Users));
		}

		public async Task<IActionResult> MissingSubscriptionOff(string id)
		{
			bool result = await SetMissingSubscriptionAsync(false, id);
			if (!result)
			{
				return NotFound();
			}
			return RedirectToAction(nameof(Users));
		}

		private async Task<bool> SetMissingSubscriptionAsync(bool v, string id)
		{
			User user = _context.User.FirstOrDefault(p => p.UserID == id);
			if (user == null)
			{
				return false;
			}
			else
			{
				user.MissingSubscription = v;
				_context.Update(user);
				await _context.SaveChangesAsync();
				var appUser = await _userManager.FindByIdAsync(user.OwnerID);
				appUser.MissingSubscription = v;
				await _userManager.UpdateAsync(appUser);
				await _webSocketMessageHandler.SendMissingSubscriptionMessageAsync(v, user.Name, _wsFactory);
				return true;
			}
		}

		// GET: Monitoring/Upload
		public IActionResult Upload()
		{
			UploadViewModel vm = new UploadViewModel
			{
				DownloadDirectories = new List<FilesDirectory>()
			};

			string donwloadpath = CustomPaths.GetDownloadPath();
			DirectoryInfo dir = new DirectoryInfo(donwloadpath);
			foreach (DirectoryInfo di in dir.GetDirectories())
			{
				FilesDirectory dd = new FilesDirectory
				{
					DirectoryName = di.Name
				};

				foreach (FileInfo fi in di.GetFiles())
				{
					dd.FileNames.Add(fi.Name);
				}
				vm.DownloadDirectories.Add(dd);
			}
			List<SelectListItem> typesItems = new List<SelectListItem>();
			foreach (DownloadType c in Enum.GetValues(typeof(DownloadType)).Cast<DownloadType>().ToList() )
			{
				typesItems.Add(new SelectListItem
				{
					Value = ((byte)c).ToString(),
					Text = c.ToString()
				});
			}
			vm.Types = typesItems;

			return View(vm);
		}

		// POST: Monitoring/Upload
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Upload(UploadViewModel model)
		{
			if (ModelState.IsValid) {
				string subpath;
				switch (model.Type)
				{
					case (int)DownloadType.BaseConfig:
						subpath = CustomPaths.GetBaseConfigPath();
						break;
					case (int)DownloadType.CollarConfig:
						subpath = CustomPaths.GetCollarConfigPath();
						break;
					case (int)DownloadType.BaseBleUpdate:
						subpath = CustomPaths.GetBaseBleUpdatePath();
						break;
					case (int)DownloadType.BaseLoraUpdate:
						subpath = CustomPaths.GetBaseLoraUpdatePath();
						break;
					case (int)DownloadType.CollarGpsUpdate:
						subpath = CustomPaths.GetCollarGPSUpdatePath();
						break;
					case (int)DownloadType.CollarLoraUpdate:
						subpath = CustomPaths.GetCollarLoraUpdatePath();
						break;
					default:
						return NotFound();						
				}

				if (!Directory.Exists(subpath))
					Directory.CreateDirectory(subpath);

				var ext = Path.GetExtension(model.File.FileName);
				var name = Path.GetFileNameWithoutExtension(model.File.FileName);
				var filefullname = $"{name}{ext}";
				if (model.Version != null)
				{
					filefullname = $"{name}-{model.Version}{ext}";
				}
				var path = Path.Combine(subpath, filefullname);
				if (System.IO.File.Exists((path)))
				{
					System.IO.File.Delete(path);
				}
				using (var stream = new FileStream(path, FileMode.Create))
				{
					await model.File.CopyToAsync(stream);
					string localUrl = CustomPaths.GetDownloadURL(model.Type, filefullname);
					await _webSocketMessageHandler.SendDownloadFile(new List<string> { localUrl }, _wsFactory, _context);
				}

				model.Version = string.Empty;
				model.Type = (int)DownloadType.None;
			}

            return RedirectToAction(nameof(Upload));
        }

		public async Task<IActionResult> SendDownloadAsync(string type, string filename ) {
			
			string localUrl = CustomPaths.GetDownloadURL(type, filename);
			await _webSocketMessageHandler.SendDownloadFile(new List<string> { localUrl }, _wsFactory, _context);
			return RedirectToAction(nameof(Upload));
		}

		public IActionResult WebSocketClient()
		{
			List<User> Users = _context.User.ToList();
			List<SelectListItem> DevicesItems = new List<SelectListItem>();
			List<Device> Devices = _context.Device.ToList();
			foreach (User u in Users)
			{
				var uGroup = new SelectListGroup { Name = u.Name };
				foreach (Device c in Devices.Where(d=>d.UserId == u.UserID).ToList())
				{
					DevicesItems.Add(new SelectListItem
					{
						Value = c.DeviceId,
						Text = c.GetPlatform + " - " + c.GetName,
						Group = uGroup
					});
				}
			}
			return View(DevicesItems);
		}

		public IActionResult SendConfig() {
			var germanGroup = new SelectListGroup { Name = "German Cars" };
			var swedishGroup = new SelectListGroup { Name = "Swedish Cars" };
			List<SelectListItem> Vehicles = new List<SelectListItem>
		{
			new SelectListItem
			{
				Value = "audi",
				Text = "Audi",
				Group = germanGroup
			},
			new SelectListItem
			{
				Value = "mercedes",
				Text = "Mercedes",
				Group = germanGroup
			},
			new SelectListItem
			{
				Value = "saab",
				Text = "Saab",
				Group = swedishGroup
			},
			new SelectListItem
			{
				Value = "volvo",
				Text = "Volvo",
				Group = swedishGroup
			}
		};
			return View(Vehicles);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

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

		public MonitoringController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, IHostingEnvironment hostingEnvironment, ICustomWebSocketMessageHandler webSocketMessageHandler, ICustomWebSocketFactory wsFactory, SignInManager<ApplicationUser> signInManager)
		{
			_context = context;
			_userManager = userManager;
			_mapper = mapper;
			_hostingEnvironment = hostingEnvironment;
			_webSocketMessageHandler = webSocketMessageHandler;
			_wsFactory = wsFactory;
			_signInManager = signInManager;
		}

		public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageIndex)
		{
			MonitoringViewModel vm = new MonitoringViewModel
			{
				WebSockets = _wsFactory.All()
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
					string localUrl = $"/device/download?type={model.Type}&filename={filefullname}";
					await _webSocketMessageHandler.SendDownloadFile(localUrl, _wsFactory, _context);
				}

				model.Version = string.Empty;
				model.Type = (int)DownloadType.None;
			}

            return RedirectToAction(nameof(Upload));
        }


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

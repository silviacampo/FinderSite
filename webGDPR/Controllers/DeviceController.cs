//dotnet aspnet-codegenerator controller -name DeviceController -async -m webGDPR.Models.Device -dc webGDPR.Data.ApplicationDbContext -namespace webGDPR.Controllers -outDir Controllers

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using webGDPR.Data;
using webGDPR.Infrastructure;
using webGDPR.Infrastructure.CustomWebSockets;
using webGDPR.Models;
using webGDPR.ViewModels;

namespace webGDPR.Controllers
{
	[Authorize]
	public class DeviceController : Controller
    {
        private readonly ApplicationDbContext _context;
		UserManager<ApplicationUser> _userManager;
		IMapper _mapper;
		ICustomWebSocketMessageHandler _webSocketMessageHandler;
		ICustomWebSocketFactory _wsFactory;

		public DeviceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, ICustomWebSocketMessageHandler webSocketMessageHandler, ICustomWebSocketFactory wsFactory)
		{
            _context = context;
			_userManager = userManager;
			_mapper = mapper;
			_webSocketMessageHandler = webSocketMessageHandler;
			_wsFactory = wsFactory;
		}

        // GET: Device
        public async Task<IActionResult> Index()
        {
			//string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;

			List<Device> devices = await _context.Device.AsNoTracking().OrderBy(d=>d.UserId).ToListAsync(); //.Where(b => b.UserId == UserId)
			return View(devices);
        }

		// /device/download
		[AllowAnonymous]
		public async Task<IActionResult> Download(int type, string filename)
		{
			string path;
			byte[] bytes;
			CancellationToken ct = new CancellationToken();

			switch (type) {
				case (int)DownloadType.GpsEphemeris:
					path = Path.Combine(CustomPaths.GetGPSEphemerisPath(), filename);					
					bytes = await System.IO.File.ReadAllBytesAsync(path, ct);
					Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");
					return new FileContentResult(bytes, "application/ubx");
				case (int)DownloadType.BaseConfig:
					path = Path.Combine(CustomPaths.GetBaseConfigPath(), filename);
					bytes = await System.IO.File.ReadAllBytesAsync(path, ct);
					Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");
					return new FileContentResult(bytes, "text/xml");
				case (int)DownloadType.CollarConfig:
					path = Path.Combine(CustomPaths.GetCollarConfigPath(), filename);
					bytes = await System.IO.File.ReadAllBytesAsync(path, ct);
					Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");
					return new FileContentResult(bytes, "text/xml");
				case (int)DownloadType.BaseBleUpdate:
					path = Path.Combine(CustomPaths.GetBaseBleUpdatePath(), filename);
					bytes = await System.IO.File.ReadAllBytesAsync(path, ct);
					Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");
					return new FileContentResult(bytes, "application/exe");
				case (int)DownloadType.BaseLoraUpdate:
					path = Path.Combine(CustomPaths.GetBaseLoraUpdatePath(), filename);
					bytes = await System.IO.File.ReadAllBytesAsync(path, ct);
					Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");
					return new FileContentResult(bytes, "application/exe");
				case (int)DownloadType.CollarGpsUpdate:
					path = Path.Combine(CustomPaths.GetCollarGPSUpdatePath(), filename);
					bytes = await System.IO.File.ReadAllBytesAsync(path, ct);
					Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");
					return new FileContentResult(bytes, "application/exe");
				case (int)DownloadType.CollarLoraUpdate:
					path = Path.Combine(CustomPaths.GetCollarLoraUpdatePath(), filename);
					bytes = await System.IO.File.ReadAllBytesAsync(path, ct);
					Response.Headers.Add("Content-Disposition", $"attachment; filename={filename}");
					return new FileContentResult(bytes, "application/exe");
				default:
					return NotFound();
			}

		}

		[AllowAnonymous]
		public IActionResult DownloadApp()
		{
			return View();
		}

		//https://stackoverflow.com/questions/44538772/asp-net-core-form-post-results-in-a-http-415-unsupported-media-type-response
		//this could be a case for patch:
		//https://www.codeproject.com/Articles/1282080/Accepting-Partial-Resources-with-Newtonsoft-Json
		[HttpPost]
		public async Task<IActionResult> BanOn([FromForm]string id)
		{
			bool result = await SetBanAsync(true, id);
			if (!result)
			{
				return NotFound();
			}
			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> BanOff([FromForm]string id)
		{
			bool result = await SetBanAsync(false, id);
			if (!result)
			{
				return NotFound();
			}
			return Ok();
		}

		private async Task<bool> SetBanAsync(bool activate, string id)
		{
			Device device = await _context.Device.FirstAsync(c => c.DeviceId == id);
			if (device == null)
			{
				return false;
			}
			else
			{
				device.Banned = activate;
				_context.Update(device);
				await _context.SaveChangesAsync();

				CustomWebSocket ws = _wsFactory.ClientByDeviceId(device.DeviceId);
					if (ws != null)
					{
						await _webSocketMessageHandler.SendDeviceBannedMessage(ws, device.Banned); //yes or not
					}				
				return true;
			}
		}

		[Authorize]
		public async Task<IActionResult> ConnectionTimeline(string id)
		{
			DeviceConnectionTimelineViewModel model = new DeviceConnectionTimelineViewModel();
			model.Device = _context.Device.Find(id);
			model.Logs = await GetConnectionTimelineAsync(id, 0);
			return View(model);
		}

		[Authorize]
		public async Task<IActionResult> ConnectionTimelineMore(string id, int page)
		{
			List<TimelineItem> list = await GetConnectionTimelineAsync(id, page);
			return new JsonResult(list);
		}

		private const int connectionTimelinePageSize = 10;
		private async Task<List<TimelineItem>> GetConnectionTimelineAsync(string id, int page) {
			List<TimelineItem> list = new List<TimelineItem>();
			List<DeviceLog> logs = await _context.DeviceLog.Include(l => l.Device).Where(l => l.DeviceId == id && l.Reason.Contains("WebSocket")).OrderByDescending(l => l.CreationDate).Skip(page * connectionTimelinePageSize).Take(connectionTimelinePageSize).ToListAsync();
			foreach (var log in logs)
			{
				list.Add(new TimelineItem()
				{
					ItemDate = log.CreationDate,
					ItemLeftTitle = log.Device.GetName,
					ItemMessage = log.Reason,
					ItemMore = log.Message,
					Orientation = (log.Reason.Contains("Add") ? TimelineItemOrientation.left : TimelineItemOrientation.right)
				});
			}
			return list;
		}

		// GET: Device/Details/5
		public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Device
                .FirstOrDefaultAsync(m => m.DeviceId == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // GET: Device/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Device/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeviceId,Type,Platform,Name,Model,Manufacturer,OSVersion,AliasName")] Device device)
        {
            if (ModelState.IsValid)
            {
				device.UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
				_context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(device);
        }

        // GET: Device/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Device.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        // POST: Device/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("DeviceId,Type,Platform,Name,Model,Manufacturer,OSVersion,AliasName,Banned,IsLogging")] Device device)
        {
            if (id != device.DeviceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
				try
				{
					var found = await _context.Device.AsNoTracking().FirstAsync(c => c.DeviceId == id);
					device.UserId = found.UserId;
					_context.Update(device);
					await _context.SaveChangesAsync();
					if (found.Banned != device.Banned) //only when value changes
					{
						CustomWebSocket ws = _wsFactory.ClientByDeviceId(device.DeviceId);
						if (ws != null)
						{
							await _webSocketMessageHandler.SendDeviceBannedMessage(ws, device.Banned); //yes or not
						}
					}
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!DeviceExists(device.DeviceId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(UserController.Dashboard), "User");
			}
            return View(device);
        }

		public async Task<IActionResult> LoggingOn(string id)
		{
			bool result = await LoggingAsync(true, id);
			if (!result)
			{
				return NotFound();
			}
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> LoggingOff(string id)
		{
			bool result = await LoggingAsync(false, id);
			if (!result)
			{
				return NotFound();
			}
			return RedirectToAction(nameof(Index));
		}

		private async Task<bool> LoggingAsync(bool v, string id)
		{
			Device device = _context.Device.FirstOrDefault(p => p.DeviceId == id);
			if (device == null)
			{
				return false;
			}
			else
			{
				device.IsLogging = v;
				_context.Update(device);
				await _context.SaveChangesAsync();
				return true;
			}
		}


		// GET: Device/Delete/5
		public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Device
                .FirstOrDefaultAsync(m => m.DeviceId == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Device/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromForm]string id)
        {
            var device = await _context.Device.FindAsync(id);
            //_context.Device.Remove(device);
			//Soft delete
			device.Deleted = true;
			_context.Device.Update(device);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
        }

        private bool DeviceExists(string id)
        {
            return _context.Device.Any(e => e.DeviceId == id);
        }
    }
}

//https://mattmillican.com/blog/aspnetcore-controller-scaffolding
//dotnet aspnet-codegenerator controller -name BaseController -async -m AgendaSignalR.Infrastructure.Base -dc webGDPR.Data.ApplicationDbContext -namespace webGDPR.Controllers -outDir Controllers
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webGDPR.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using webGDPR.ViewModels;
using AutoMapper;
using webGDPR.Models;
using webGDPR.Infrastructure.CustomWebSockets;
using webGDPR.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using webGDPR.Hubs;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace webGDPR.Controllers
{
	[Authorize]
	public class BaseController : Controller
    {
        private readonly ApplicationDbContext _context;
		UserManager<ApplicationUser> _userManager;
		IMapper _mapper;
		ICustomWebSocketMessageHandler _webSocketMessageHandler;
		private readonly ICustomWebSocketFactory _wsFactory;
		private readonly IHubContext<BroadcastHub> _hubContext;
		private readonly ILogger<BaseController> _logger;

		public BaseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, ICustomWebSocketMessageHandler webSocketMessageHandler,ICustomWebSocketFactory wsFactory, IHubContext<BroadcastHub> hubContext, ILogger<BaseController> logger)
		{
            _context = context;
			_userManager = userManager;
			_mapper = mapper;
			_webSocketMessageHandler = webSocketMessageHandler;
			_wsFactory = wsFactory;
			_hubContext = hubContext;
			_logger = logger;
		}

		public async Task SendToAllAsync(string action, Base @base)
		{
			string message = string.Empty;
			string json = JsonConvert.SerializeObject(@base);
			//TODO: localize
			if (action == nameof(Edit))
			{
				message = $"{@base.Name} modified.";
			}
			else if (action == nameof(Create))
			{
				message = $"{@base.Name} created.";
			}
			else if (action == nameof(Delete))
			{
				message = $"{@base.Name} deleted.";
			}

			await _hubContext.Clients.Group(_userManager.GetUserName(User)).SendAsync("ReceiveMessage", _userManager.GetUserName(User), message, json);
		}

		// GET: Base
		public async Task<IActionResult> Index()
        {
			List<BaseViewModel> model = new List<BaseViewModel>();
			//string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;

			List<Base> bases = await _context.Base.AsNoTracking().Include(b => b.LastStatus).ThenInclude(c=>c.DeviceConnectedTo).OrderBy(b => b.UserId).ToListAsync();
			foreach (var b in bases)
			{
				BaseViewModel mb = _mapper.Map<BaseViewModel>(new Tuple<Base, BaseStatus>(b, b.LastStatus));
				if (b.LastStatus != null && b.LastStatus.IsConnected)
				{
					mb.DeviceConnectedTo = b.LastStatus.DeviceConnectedTo;
				}
				model.Add(mb);
			}

			return View(model);
        }

        // GET: Base/Details/5
        public async Task<IActionResult> Details(string id, string searchString, string currentFilter, int? pageIndex)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @base = await _context.Base.Include(b => b.LastStatus).ThenInclude(c => c.DeviceConnectedTo).FirstOrDefaultAsync(m => m.BaseId == id && !m.Deleted);
            if (@base == null)
            {
                return NotFound();
            }

			BaseViewModel model = _mapper.Map<BaseViewModel>(new Tuple<Base, BaseStatus>(@base, @base.LastStatus));
			if (@base.LastStatus != null && @base.LastStatus.IsConnected)
			{
				model.DeviceConnectedTo = @base.LastStatus.DeviceConnectedTo;
			}

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
			model.CurrentFilter = searchString;
			int pageSize = 10;
			model.BaseStatus = await PaginatedList<BaseStatus>.CreateAsync(
				_context.BaseStatus.Include(s=>s.DeviceConnectedTo).Where(s => s.BaseId == id && s.ConnectedTo.Contains(searchString)).OrderByDescending(d => d.CreationDate).AsNoTracking(), pageIndex ?? 1, pageSize);

			return View(model);
        }

		[Authorize]
		public async Task<IActionResult> Timeline(string id, string parameter)
		{
			if (id == null)
			{
				return NotFound();
			}

			var @base = await _context.Base.Include(b => b.LastStatus).ThenInclude(c => c.DeviceConnectedTo).FirstOrDefaultAsync(m => m.BaseId == id && !m.Deleted);
			if (@base == null)
			{
				return NotFound();
			}

			BaseTimelineViewModel model = new BaseTimelineViewModel
			{
				Base = @base,
				Parameter = parameter,
				Logs = await GetConnectionTimelineAsync(id, 0, parameter)
			};
			return View(model);
		}

		[Authorize]
		public async Task<IActionResult> TimelineMore(string id, int page, string parameter)
		{
			List<TimelineItem> list = await GetConnectionTimelineAsync(id, page, parameter);
			return new JsonResult(list);
		}

		private const int connectionTimelinePageSize = 10;
		private async Task<List<TimelineItem>> GetConnectionTimelineAsync(string id, int page, string parameter)
		{
			List<TimelineItem> list = new List<TimelineItem>();
			List<BaseStatus> logs = await _context.BaseStatus.Include(l => l.DeviceConnectedTo).Where(s => s.BaseId == id).OrderByDescending(l => l.CreationDate).Skip(page * connectionTimelinePageSize).Take(connectionTimelinePageSize).ToListAsync();
			foreach (var log in logs)
			{
				string itemMessage = string.Empty;
				string itemMore = Newtonsoft.Json.JsonConvert.SerializeObject(log);
				TimelineItemOrientation itemOrientation =TimelineItemOrientation.left;
				if (parameter == "IsConnected") {
					if (log.IsConnected == true)
					{
						itemMessage = "Connected";
					}
					else
					{
						itemMessage = "Disconnected";
					}
					if (log.IsConnected == true)
					{
						itemOrientation = TimelineItemOrientation.left;
					}
					else
					{
						itemOrientation = TimelineItemOrientation.right;
					}
				} else if (parameter == "IsPlugged")
				{
					if (log.IsPlugged == true)
					{
						itemMessage = "Plugged";
					}
					else
					{
						itemMessage = "Unplugged";
					}
					if (log.IsPlugged == true)
					{
						itemOrientation = TimelineItemOrientation.left;
					}
					else
					{
						itemOrientation = TimelineItemOrientation.right;
					}
				} else if (parameter == "Radio")
				{
					itemMessage = "Radio: " + log.RadioPercentage;

					if (log.Radio < 40)
					{
						itemOrientation = TimelineItemOrientation.left;
					}
					else
					{
						itemOrientation = TimelineItemOrientation.right;
					}
				}
				else if (parameter == "Battery")
				{
					itemMessage = "Battery: " + log.Battery.ToString() + "%";
					if (log.HasBattery) {
						itemMessage += " - Has a battery";
					}
					if (log.IsCharging)
					{
						itemMessage += " - Is charging a battery";
					}
					if (log.Battery < 25)
					{
						itemOrientation = TimelineItemOrientation.left;
					}
					else
					{
						itemOrientation = TimelineItemOrientation.right;
					}
				}
				list.Add(new TimelineItem()
				{
					ItemDate = log.CreationDate,
					ItemLeftTitle = log.DeviceConnectedTo?.GetName,
					ItemMessage = itemMessage,
					ItemMore = itemMore,
					Orientation = itemOrientation
				});
			}
			return list;
		}

		// GET: Base/Create
		public IActionResult Create()
        {
            return View();
        }

        // POST: Base/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BaseId,HWId,Name,Text,Description")] Base @base)
        {
            if (ModelState.IsValid)
            {
				@base.UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
				try
				{
					@base.BaseNumber = Convert.ToByte(_context.Base.Where(b => b.UserId == @base.UserId).Count() + 1);
				}
				catch (Exception e) {
					_logger.LogError("BaseController - Create Error:" + e.Message);
					@base.BaseNumber = 0;
				}
				_context.Add(@base);
                await _context.SaveChangesAsync();
				await SendToAllAsync(nameof(Create), @base);
				//send message to connected devices
				Infrastructure.CustomWebSockets.Messages.Base ba = _mapper.Map<Infrastructure.CustomWebSockets.Messages.Base>(@base);
				await _webSocketMessageHandler.SendBaseAsync(ba, _userManager.GetUserName(User), _wsFactory);
				return RedirectToAction(nameof(UserController.Dashboard), "User");
			}
            return View(@base);
        }

        // GET: Base/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @base = await _context.Base.FirstOrDefaultAsync(c=>c.BaseId == id && !c.Deleted);
            if (@base == null)
            {
                return NotFound();
            }
            return View(@base);
        }

        // POST: Base/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BaseId,HWId,Name,Text,Description")] Base @base)
        {
            if (id != @base.BaseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					var found = await _context.Base.AsNoTracking().FirstOrDefaultAsync(c=>c.BaseId == id && !c.Deleted);
					@base.UserId = found.UserId;
					@base.BaseNumber = found.BaseNumber;
					_context.Update(@base);
                    await _context.SaveChangesAsync();
					//send message to connected devices
					Infrastructure.CustomWebSockets.Messages.BaseCore bc = _mapper.Map<Infrastructure.CustomWebSockets.Messages.BaseCore>(@base);
					await _webSocketMessageHandler.SendBaseCoreAsync(bc, _userManager.GetUserName(User), _wsFactory);
				}
				catch (DbUpdateConcurrencyException)
                {
                    if (!BaseExists(@base.BaseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
				await SendToAllAsync(nameof(Edit), @base);
				return RedirectToAction(nameof(UserController.Dashboard), "User");
			}
            return View(@base);
        }

        // GET: Base/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
			if (id == null)
			{
				return NotFound();
			}

			var @base = await _context.Base.Include(b => b.LastStatus).ThenInclude(c => c.DeviceConnectedTo).FirstOrDefaultAsync(m => m.BaseId == id && !m.Deleted);
			if (@base == null)
			{
				return NotFound();
			}

			BaseViewModel model = _mapper.Map<BaseViewModel>(new Tuple<Base, BaseStatus>(@base, @base.LastStatus));
			if (@base.LastStatus != null && @base.LastStatus.IsConnected)
			{
				model.DeviceConnectedTo = @base.LastStatus.DeviceConnectedTo;
			}

			return View(model);

		}

        // POST: Base/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromForm]string id)
        {
            var @base = await _context.Base.FindAsync(id);
            //_context.Base.Remove(@base);
			//Soft delete
			@base.Deleted = true;
			_context.Base.Update(@base);
			await _context.SaveChangesAsync();
			await SendToAllAsync(nameof(Delete), @base);
			//send message to connected devices
			await _webSocketMessageHandler.SendDeletedBaseAsync(@base.BaseNumber, _userManager.GetUserName(User), _wsFactory);

			return RedirectToAction(nameof(Index));
        }

        private bool BaseExists(string id)
        {
            return _context.Base.Any(e => e.BaseId == id && !e.Deleted);
        }
    }
}

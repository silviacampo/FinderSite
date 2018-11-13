//dotnet aspnet-codegenerator controller -name CollarController -asyn c -m AgendaSignalR.Infrastructure.Collar -dc webGDPR.Data.ApplicationDbContext -namespace webGDPR.Controllers -outDir Controllers

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webGDPR.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using webGDPR.ViewModels;
using webGDPR.Models;
using webGDPR.Infrastructure.CustomWebSockets;
using webGDPR.Infrastructure;

namespace webGDPR.Controllers
{
	[Authorize]
	public class CollarController : Controller
    {
        private readonly ApplicationDbContext _context;
		UserManager<ApplicationUser> _userManager;
		IMapper _mapper;
		ICustomWebSocketMessageHandler _webSocketMessageHandler;
		ICustomWebSocketFactory _wsFactory;

		public CollarController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, ICustomWebSocketMessageHandler webSocketMessageHandler, ICustomWebSocketFactory wsFactory)
		{
            _context = context;
			_userManager = userManager;
			_mapper = mapper;
			_webSocketMessageHandler = webSocketMessageHandler;
			_wsFactory = wsFactory;
		}

        // GET: Collar
        public async Task<IActionResult> Index()
        {
			List<CollarViewModel> model = new List<CollarViewModel>();
			string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;

			List<Collar> collars = await _context.Collar.AsNoTracking().Where(b => b.UserId == UserId && !b.Deleted).Include(b => b.LastStatus).ThenInclude(c => c.BaseConnectedTo).ToListAsync();
			foreach (var c in collars)
			{
				CollarViewModel cvm = _mapper.Map<CollarViewModel>(new Tuple<Collar, CollarStatus>(c, c.LastStatus));
				if (c.LastStatus != null)
				{
					cvm.BaseConnectedTo = c.LastStatus.BaseConnectedTo;
				}
				model.Add(cvm);
			}

			return View(model);
        }

        // GET: Collar/Details/5
        public async Task<IActionResult> Details(string id, string searchString, string currentFilter, int? pageIndex)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collar = await _context.Collar.Include(b => b.LastStatus).ThenInclude(c => c.BaseConnectedTo).FirstOrDefaultAsync(m => m.CollarId == id && !m.Deleted);
            if (collar == null)
            {
                return NotFound();
            }

			CollarViewModel model = _mapper.Map<CollarViewModel>(new Tuple<Collar, CollarStatus>(collar, collar.LastStatus));
			if (collar.LastStatus != null)
			{
				model.BaseConnectedTo = collar.LastStatus.BaseConnectedTo;
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
			model.CollarStatus = await PaginatedList<CollarStatus>.CreateAsync(
				_context.CollarStatus.Where(s => s.CollarId == id && s.ConnectedTo.Contains(searchString)).OrderByDescending(d => d.CreationDate).AsNoTracking(), pageIndex ?? 1, pageSize);

			return View(model);
		}

        // GET: Collar/Create
        public async Task<IActionResult> Create()
        {
			string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
			List<string> petCollars = await _context.PetCollar.Where(p => p.IsActive && p.CollarId != null).Select(c => c.PetId).ToListAsync();
			List<Pet> pets = await _context.Pet.AsNoTracking().Where(b => b.UserId == UserId && !b.Deleted && !petCollars.Contains(b.PetId)).ToListAsync();

			EditCollarViewModel model = new EditCollarViewModel();
			List<SelectListItem> petsItems = new List<SelectListItem>();
			foreach (Pet c in pets)
			{
				petsItems.Add(new SelectListItem
				{
					Value = c.PetId,
					Text = c.Name
				});
			}

			model.Pets = petsItems;
			return View(model);
		}

        // POST: Collar/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CollarId,HWId,Name,Description,PetId")] EditCollarViewModel collar)
        {
            if (ModelState.IsValid)
            {
				Collar c = _mapper.Map<Collar>(collar);
				c.UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
				try
				{
					c.BaseNumber = Convert.ToByte(_context.Base.Where(b => b.UserId == collar.UserId).Count());
				}
				catch (Exception e)
				{
					c.BaseNumber = 0;
				}
				try
				{
					c.CollarNumber = Convert.ToByte(_context.Collar.Where(b => b.UserId == collar.UserId).Count() +1);
				}
				catch (Exception e)
				{
					c.CollarNumber = 0;
				}
				_context.Add(c);
                await _context.SaveChangesAsync();
				if (collar.PetId != null)
				{
					PetCollar pc = new PetCollar
					{
						PetId = collar.PetId,
						CollarId = c.CollarId,
						StartDate = DateTime.Now,
						CreationDate = DateTime.Now,
						IsActive = true,
						UserId = c.UserId
					};
					_context.Add(pc);

					var pet = await _context.Pet
					.FirstOrDefaultAsync(m => m.PetId == collar.PetId && !m.Deleted);
					pet.LastCollarId = pc.PetCollarId;
					_context.Update(pet);

					await _context.SaveChangesAsync();
				}
				//send message to connected devices
				Infrastructure.CustomWebSockets.Messages.Collar co = _mapper.Map<Infrastructure.CustomWebSockets.Messages.Collar>(c);
				await _webSocketMessageHandler.SendCollarAsync(co, _userManager.GetUserName(User), _wsFactory);
				return RedirectToAction(nameof(Index));
            }
            return View(collar);
        }

        // GET: Collar/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collar = await _context.Collar.FirstOrDefaultAsync(c=>c.CollarId == id && !c.Deleted);
            if (collar == null)
            {
                return NotFound();
            }

			string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
			List<string> petCollars = await _context.PetCollar.Where(p => p.IsActive && p.CollarId != collar.CollarId).Select(c => c.PetId).ToListAsync();
			List<Pet> pets = await _context.Pet.AsNoTracking().Where(b => b.UserId == UserId && !b.Deleted && !petCollars.Contains(b.PetId)).ToListAsync();

			EditCollarViewModel model = new EditCollarViewModel();
			model = _mapper.Map<EditCollarViewModel>(collar);
			model.PetId = _context.PetCollar.FirstOrDefault(f => f.IsActive && f.CollarId == collar.CollarId).PetId;
			List<SelectListItem> petsItems = new List<SelectListItem>();
			foreach (Pet c in pets)
			{
				petsItems.Add(new SelectListItem
				{
					Value = c.PetId,
					Text = c.Name
				});
			}

			model.Pets = petsItems;
			return View(model);
        }

        // POST: Collar/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CollarId,HWId,Name,Description,PetId")] EditCollarViewModel collar)
        {
            if (id != collar.CollarId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					Collar c = _mapper.Map<Collar>(collar);
					var found = await _context.Collar.AsNoTracking().FirstOrDefaultAsync(f => f.CollarId == id && !f.Deleted);
					c.UserId = found.UserId;
					c.CollarNumber = found.CollarNumber;
					c.BaseNumber = found.BaseNumber;
					_context.Update(c);
					await _context.SaveChangesAsync();

					PetCollar currentpet = _context.PetCollar.FirstOrDefault(f => f.CollarId == c.CollarId && f.IsActive);

					if (collar.PetId != currentpet.PetId)
					{
						currentpet.IsActive = false;

						PetCollar pc = new PetCollar
						{
							PetId = collar.PetId,
							CollarId = c.CollarId,
							StartDate = DateTime.Now,
							CreationDate = DateTime.Now,
							IsActive = true,
							UserId = c.UserId
						};
						_context.Add(pc);

						Pet cpet = _context.Pet.FirstOrDefault(g => g.PetId == currentpet.PetId);
						cpet.LastCollarId = null;
						_context.Update(cpet);

						Pet pet = _context.Pet.FirstOrDefault(g => g.PetId == collar.PetId);
						pet.LastCollarId = pc.PetCollarId;
						_context.Update(pet);

						await _context.SaveChangesAsync();
					}
					//send message to connected devices
					Infrastructure.CustomWebSockets.Messages.CollarCore cc = _mapper.Map<Infrastructure.CustomWebSockets.Messages.CollarCore>(c);
					await _webSocketMessageHandler.SendCollarCoreAsync(cc, _userManager.GetUserName(User), _wsFactory);
				}
				catch (DbUpdateConcurrencyException)
                {
                    if (!CollarExists(collar.CollarId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(collar);
        }

        // GET: Collar/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
			if (id == null)
			{
				return NotFound();
			}

			var collar = await _context.Collar.Include(b => b.LastStatus).ThenInclude(c => c.BaseConnectedTo).FirstOrDefaultAsync(m => m.CollarId == id && !m.Deleted);
			if (collar == null)
			{
				return NotFound();
			}

			CollarViewModel model = _mapper.Map<CollarViewModel>(new Tuple<Collar, CollarStatus>(collar, collar.LastStatus));
			if (collar.LastStatus != null)
			{
				model.BaseConnectedTo = collar.LastStatus.BaseConnectedTo;
			}

			return View(model);
		}

        // POST: Collar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var collar = await _context.Collar.FindAsync(id);
			//_context.Collar.Remove(collar);
			//soft delete
			collar.Deleted = true;
			_context.Update(collar);
			var petCollar = _context.PetCollar.FirstOrDefault(c => c.CollarId == id && c.IsActive);
			petCollar.IsActive = false;
			petCollar.EndDate = DateTime.Now;
			_context.Update(petCollar);
			var pet = _context.Pet.FirstOrDefault(c => c.PetId == petCollar.PetId);
			pet.LastCollarId = null;
			_context.Update(pet);
			await _context.SaveChangesAsync();
			//send message to connected devices
			await _webSocketMessageHandler.SendDeletedCollarAsync(collar.CollarNumber, _userManager.GetUserName(User), _wsFactory);
			return RedirectToAction(nameof(Index));
        }

        private bool CollarExists(string id)
        {
            return _context.Collar.Any(e => e.CollarId == id && !e.Deleted);
        }
    }
}

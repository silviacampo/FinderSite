//dotnet aspnet-codegenerator controller -name CollarController -asyn c -m AgendaSignalR.Infrastructure.Collar -dc webGDPR.Data.ApplicationDbContext -namespace webGDPR.Controllers -outDir Controllers

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AgendaSignalR.Infrastructure;
using webGDPR.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using webGDPR.ViewModels;
using webGDPR.Models;

namespace webGDPR.Controllers
{
	[Authorize]
	public class CollarController : Controller
    {
        private readonly ApplicationDbContext _context;
		UserManager<ApplicationUser> _userManager;
		IMapper _mapper;

		public CollarController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
		{
            _context = context;
			_userManager = userManager;
			_mapper = mapper;
		}

        // GET: Collar
        public async Task<IActionResult> Index()
        {
			List<CollarViewModel> model = new List<CollarViewModel>();
			string UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;

			List<Collar> collars = await _context.Collar.AsNoTracking().Where(b => b.UserId == UserId).Include(b => b.LastStatus).ThenInclude(c => c.BaseConnectedTo).ToListAsync();
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
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collar = await _context.Collar.Include(b => b.LastStatus).ThenInclude(c => c.BaseConnectedTo).FirstOrDefaultAsync(m => m.CollarId == id);
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

        // GET: Collar/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Collar/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CollarId,HWId,Name,Description")] Collar collar)
        {
            if (ModelState.IsValid)
            {
				collar.UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
				try
				{
					collar.BaseNumber = Convert.ToByte(_context.Base.Where(b => b.UserId == collar.UserId).Count());
				}
				catch (Exception e)
				{
					collar.BaseNumber = 0;
				}
				try
				{
					collar.CollarNumber = Convert.ToByte(_context.Collar.Where(b => b.UserId == collar.UserId).Count() +1);
				}
				catch (Exception e)
				{
					collar.CollarNumber = 0;
				}
				_context.Add(collar);
                await _context.SaveChangesAsync();
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

            var collar = await _context.Collar.FindAsync(id);
            if (collar == null)
            {
                return NotFound();
            }
            return View(collar);
        }

        // POST: Collar/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CollarId,HWId,Name,Description")] Collar collar)
        {
            if (id != collar.CollarId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					var found = await _context.Collar.AsNoTracking().FirstAsync(c => c.CollarId == id);
					collar.UserId = found.UserId;
					collar.CollarNumber = found.CollarNumber;
					collar.BaseNumber = found.BaseNumber;
					_context.Update(collar);
                    await _context.SaveChangesAsync();
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

			var collar = await _context.Collar.Include(b => b.LastStatus).ThenInclude(c => c.BaseConnectedTo).FirstOrDefaultAsync(m => m.CollarId == id);
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
            _context.Collar.Remove(collar);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollarExists(string id)
        {
            return _context.Collar.Any(e => e.CollarId == id);
        }
    }
}

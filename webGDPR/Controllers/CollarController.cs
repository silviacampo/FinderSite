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

namespace webGDPR.Controllers
{
	[Authorize]
	public class CollarController : Controller
    {
        private readonly ApplicationDbContext _context;
		UserManager<ApplicationUser> _userManager;

		public CollarController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
			_userManager = userManager;
		}

        // GET: Collar
        public async Task<IActionResult> Index()
        {
            return View(await _context.Collar.ToListAsync());
        }

        // GET: Collar/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collar = await _context.Collar
                .FirstOrDefaultAsync(m => m.CollarId == id);
            if (collar == null)
            {
                return NotFound();
            }

            return View(collar);
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
        public async Task<IActionResult> Create([Bind("CollarId,HWId,Name,IsConnected,IsGPSConnected,Battery,Radio,Description,UserId")] Collar collar)
        {
            if (ModelState.IsValid)
            {
				collar.UserId = _userManager.GetUserId(User);
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
        public async Task<IActionResult> Edit(string id, [Bind("CollarId,HWId,Name,IsConnected,IsGPSConnected,Battery,Radio,Description,UserId")] Collar collar)
        {
            if (id != collar.CollarId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

            var collar = await _context.Collar
                .FirstOrDefaultAsync(m => m.CollarId == id);
            if (collar == null)
            {
                return NotFound();
            }

            return View(collar);
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

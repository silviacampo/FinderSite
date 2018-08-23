//https://mattmillican.com/blog/aspnetcore-controller-scaffolding
//dotnet aspnet-codegenerator controller -name BaseController -async -m AgendaSignalR.Infrastructure.Base -dc webGDPR.Data.ApplicationDbContext -namespace webGDPR.Controllers -outDir Controllers
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
	public class BaseController : Controller
    {
        private readonly ApplicationDbContext _context;
		UserManager<ApplicationUser> _userManager;


		public BaseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
			_userManager = userManager;
        }

        // GET: Base
        public async Task<IActionResult> Index()
        {
			return View(await _context.Base.ToListAsync());
        }

        // GET: Base/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @base = await _context.Base
                .FirstOrDefaultAsync(m => m.BaseId == id);
            if (@base == null)
            {
                return NotFound();
            }

            return View(@base);
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
        public async Task<IActionResult> Create([Bind("BaseId,HWId,Name,IsConnected,IsPlugged,IsCharging,Battery,HasBattery,Radio,Text,Description,UserId")] Base @base)
        {
            if (ModelState.IsValid)
            {
				@base.UserId = _context.User.FirstOrDefault(u => u.OwnerID == _userManager.GetUserId(User)).UserID;
				_context.Add(@base);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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

            var @base = await _context.Base.FindAsync(id);
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
        public async Task<IActionResult> Edit(string id, [Bind("BaseId,HWId,Name,IsConnected,IsPlugged,IsCharging,Battery,HasBattery,Radio,Text,Description,UserId")] Base @base)
        {
            if (id != @base.BaseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@base);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
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

            var @base = await _context.Base
                .FirstOrDefaultAsync(m => m.BaseId == id);
            if (@base == null)
            {
                return NotFound();
            }

            return View(@base);
        }

        // POST: Base/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var @base = await _context.Base.FindAsync(id);
            _context.Base.Remove(@base);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BaseExists(string id)
        {
            return _context.Base.Any(e => e.BaseId == id);
        }
    }
}

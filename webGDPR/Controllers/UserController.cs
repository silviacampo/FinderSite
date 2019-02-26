using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webGDPR.Authorization;
using webGDPR.Data;
using webGDPR.Infrastructure.CustomWebSockets;
using webGDPR.Models;

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
			foreach (Collar c in user.Collars) {
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
		public async Task<IActionResult> PickLocation()
		{
			User user = await _context.User.FirstOrDefaultAsync(u => u.OwnerID == _userManager.GetUserId(User));
			return View(user);
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
